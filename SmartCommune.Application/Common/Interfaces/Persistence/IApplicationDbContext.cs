using Microsoft.EntityFrameworkCore;

using SmartCommune.Domain.DossierAggregate;
using SmartCommune.Domain.DossierAggregate.Entities;
using SmartCommune.Domain.MenuItemAggregate;
using SmartCommune.Domain.MenuItemAggregate.Entities;
using SmartCommune.Domain.NotificationAggregate;
using SmartCommune.Domain.NotificationAggregate.Entities;
using SmartCommune.Domain.PermissionAggregate;
using SmartCommune.Domain.PlanAggregate;
using SmartCommune.Domain.PlanAggregate.Entities;
using SmartCommune.Domain.RoleAggregate;
using SmartCommune.Domain.RoleAggregate.Entities;
using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.Entities;
using SmartCommune.Domain.WorkItemAggregate;
using SmartCommune.Domain.WorkItemAggregate.Entities;
using SmartCommune.Domain.WorkspaceAggregate;
using SmartCommune.Domain.WorkspaceAggregate.Entities;

namespace SmartCommune.Application.Common.Interfaces.Persistence;

public interface IApplicationDbContext
{
    // User Aggregate.
    DbSet<ApplicationUser> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    // Role Aggregate.
    DbSet<Role> Roles { get; }
    DbSet<RolePermission> RolePermissions { get; }

    // Menu Item Aggregate.
    DbSet<MenuItem> MenuItems { get; }
    DbSet<MenuItemPermission> MenuItemPermissions { get; }

    // Permission Aggregate.
    DbSet<Permission> Permissions { get; }

    // Notification Aggregate.
    DbSet<Notification> Notifications { get; }
    DbSet<NotificationAttachment> NotificationAttachments { get; }
    DbSet<UserNotification> UserNotifications { get; }

    // Work Item Aggregate.
    DbSet<WorkItem> WorkItems { get; }
    DbSet<WorkItemAttachment> WorkItemAttachments { get; }
    DbSet<WorkItemMember> WorkItemMembers { get; }
    DbSet<WorkItemReport> WorkItemReports { get; }
    DbSet<WorkItemReportAttachment> WorkItemReportAttachments { get; }
    DbSet<WorkItemComment> WorkItemComments { get; }

    // Workspace Aggregate.
    DbSet<Workspace> Workspaces { get; }
    DbSet<WorkItemCollection> WorkItemCollections { get; }

    // Plan Aggregate.
    DbSet<Plan> Plans { get; }
    DbSet<PlanEvaluation> PlanEvaluations { get; }

    // Document Aggregate.
    DbSet<Dossier> Documents { get; }
    DbSet<DossierAttachment> DocumentAttachments { get; }
    DbSet<DossierMember> DocumentMembers { get; }

    // SaveChangesAsync của DbContext đã có sẵn, nên không cần triển khai, chỉ khai báo để dùng là đủ.
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}