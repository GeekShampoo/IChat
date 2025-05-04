using IChat.Domain.Entities;
using IChat.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IChat.Server.Infrastructure.Data
{
    /// <summary>
    /// IChat 应用程序的数据库上下文
    /// </summary>
    public class IChatDbContext : DbContext
    {
        public IChatDbContext(DbContextOptions<IChatDbContext> options) : base(options) { }

        // 实体集合
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<UserLoginLog> UserLoginLogs { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageReadReceipt> MessageReadReceipts { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 添加全局查询过滤器以支持软删除
            ApplySoftDeleteQueryFilter(modelBuilder);

            // 配置实体关系和约束
            ConfigureUserEntities(modelBuilder);
            ConfigureMessageEntities(modelBuilder);
            ConfigureGroupEntities(modelBuilder);
            ConfigureFriendshipEntities(modelBuilder);
            ConfigureConversationEntities(modelBuilder);
            ConfigureFileEntities(modelBuilder);
            ConfigureNotificationEntities(modelBuilder);
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        #region Private Methods

        private void UpdateAuditFields()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                // 针对新增实体设置创建时间
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                }
                // 针对更新实体设置更新时间
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
                // 针对删除的实体应用软删除
                else if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = now;
                }
            }
        }

        private void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
        {
            // 为所有继承自 BaseEntity 的实体添加全局查询过滤器
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                    var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var falseConstant = System.Linq.Expressions.Expression.Constant(false);
                    var expression = System.Linq.Expressions.Expression.Equal(property, falseConstant);
                    var lambda = System.Linq.Expressions.Expression.Lambda(expression, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        private void ConfigureUserEntities(ModelBuilder modelBuilder)
        {
            // 用户实体配置
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                // 设置用户与用户令牌的一对多关系
                entity.HasMany(u => u.Tokens)
                      .WithOne(t => t.User)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // 设置用户与用户设置的一对一关系
                entity.HasOne(u => u.Setting)
                      .WithOne(s => s.User)
                      .HasForeignKey<UserSetting>(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // 设置用户与登录日志的一对多关系
                entity.HasMany(u => u.LoginLogs)
                      .WithOne(l => l.User)
                      .HasForeignKey(l => l.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // 设置用户与设备的一对多关系
                entity.HasMany(u => u.Devices)
                      .WithOne(d => d.User)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureMessageEntities(ModelBuilder modelBuilder)
        {
            // 消息实体配置
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // 设置消息与发送者的多对一关系
                entity.HasOne(m => m.Sender)
                      .WithMany(u => u.SentMessages)
                      .HasForeignKey(m => m.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 设置消息与接收者的多对一关系（私聊）
                entity.HasOne(m => m.Recipient)
                      .WithMany(u => u.ReceivedMessages)
                      .HasForeignKey(m => m.RecipientId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 设置消息与群组的多对一关系（群聊）
                entity.HasOne(m => m.Group)
                      .WithMany(g => g.Messages)  // 指定 Group.Messages 作为导航属性
                      .HasForeignKey(m => m.GroupId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 设置消息与已读回执的一对多关系
                entity.HasMany(m => m.ReadReceipts)
                      .WithOne(r => r.Message)
                      .HasForeignKey(r => r.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);

                // 设置消息回复关系
                entity.HasOne(m => m.ReplyToMessage)
                      .WithMany(m => m.Replies)
                      .HasForeignKey(m => m.ReplyToMessageId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureGroupEntities(ModelBuilder modelBuilder)
        {
            // 群组实体配置
            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Id);

                // 设置群组与创建者的多对一关系
                entity.HasOne(g => g.Creator)
                      .WithMany(u => u.CreatedGroups)
                      .HasForeignKey(g => g.CreatorId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 设置群组与群成员的一对多关系
                entity.HasMany(g => g.Members)
                      .WithOne(m => m.Group)
                      .HasForeignKey(m => m.GroupId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 群成员实体配置
            modelBuilder.Entity<GroupMember>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.GroupId, e.UserId }).IsUnique();

                // 设置群成员与用户的多对一关系
                entity.HasOne(gm => gm.User)
                      .WithMany(u => u.GroupMemberships)
                      .HasForeignKey(gm => gm.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureFriendshipEntities(ModelBuilder modelBuilder)
        {
            // 好友关系实体配置
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.InitiatorId, e.RecipientId }).IsUnique();

                // 设置与发起者的关系
                entity.HasOne(f => f.Initiator)
                      .WithMany(u => u.FriendshipsInitiated)
                      .HasForeignKey(f => f.InitiatorId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 设置与接收者的关系
                entity.HasOne(f => f.Recipient)
                      .WithMany(u => u.FriendshipsReceived)
                      .HasForeignKey(f => f.RecipientId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureConversationEntities(ModelBuilder modelBuilder)
        {
            // 会话实体配置
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(e => e.Id);

                // 设置会话与拥有者的多对一关系
                entity.HasOne(c => c.Owner)
                      .WithMany(u => u.Conversations)
                      .HasForeignKey(c => c.OwnerId)
                      .OnDelete(DeleteBehavior.Cascade);

                // 设置会话与最后一条消息的多对一关系
                entity.HasOne(c => c.LastMessage)
                      .WithMany()
                      .HasForeignKey(c => c.LastMessageId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void ConfigureFileEntities(ModelBuilder modelBuilder)
        {
            // 文件附件实体配置
            modelBuilder.Entity<FileAttachment>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // 设置文件与上传者的多对一关系
                entity.HasOne(f => f.Uploader)
                      .WithMany(u => u.UploadedFiles)
                      .HasForeignKey(f => f.UploaderId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureNotificationEntities(ModelBuilder modelBuilder)
        {
            // 通知实体配置
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // 设置通知与接收者的多对一关系
                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        #endregion
    }
}