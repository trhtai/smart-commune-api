using System.Reflection;

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

namespace SmartCommune.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    // User Aggregate.
    public DbSet<ApplicationUser> Users { get; set; } = null!;
    public DbSet<UserPermission> UserPermissions { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    // Role Aggregate.
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;

    // Menu Item Aggregate.
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<MenuItemPermission> MenuItemPermissions { get; set; } = null!;

    // Permission Aggregate.
    public DbSet<Permission> Permissions { get; set; } = null!;

    // Notification Aggregate.
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationAttachment> NotificationAttachments { get; set; } = null!;
    public DbSet<UserNotification> UserNotifications { get; set; } = null!;

    // Work Item Aggregate.
    public DbSet<WorkItem> WorkItems { get; set; } = null!;
    public DbSet<WorkItemAttachment> WorkItemAttachments { get; set; } = null!;
    public DbSet<WorkItemMember> WorkItemMembers { get; set; } = null!;
    public DbSet<WorkItemReport> WorkItemReports { get; set; } = null!;
    public DbSet<WorkItemReportAttachment> WorkItemReportAttachments { get; set; } = null!;
    public DbSet<WorkItemComment> WorkItemComments { get; set; } = null!;

    // Workspace Aggregate.
    public DbSet<Workspace> Workspaces { get; set; } = null!;
    public DbSet<WorkItemCollection> WorkItemCollections { get; set; } = null!;

    // Plan Aggregate.
    public DbSet<Plan> Plans { get; set; } = null!;
    public DbSet<PlanEvaluation> PlanEvaluations { get; set; } = null!;

    // Document Aggregate.
    public DbSet<Dossier> Documents { get; set; } = null!;
    public DbSet<DossierAttachment> DocumentAttachments { get; set; } = null!;
    public DbSet<DossierMember> DocumentMembers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Áp dụng toàn bộ cấu hình từ assembly hiện tại.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}