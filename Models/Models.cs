namespace JudjodApi.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public string? Issues { get; set; }
    public string? Priority { get; set; }
    public int Progress { get; set; }
    public int SortOrder { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public User? CreatedByUser { get; set; }
    public ICollection<ProjectOwner> Owners { get; set; } = new List<ProjectOwner>();
    public ICollection<ProjectGroupAssignment> Groups { get; set; } = new List<ProjectGroupAssignment>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
}

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string? LastName { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Role { get; set; }
    public string? WorkLocation { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ProfileImagePath { get; set; }
    public bool ProjectAccessSuspended { get; set; }
    public bool ChatEnabled { get; set; }
}

public class ProjectOwner
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public Project? Project { get; set; }
    public User? User { get; set; }
}

public class ProjectGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<ProjectGroupAssignment> ProjectAssignments { get; set; } = new List<ProjectGroupAssignment>();
}

public class ProjectGroupAssignment
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int GroupId { get; set; }
    public Project Project { get; set; } = null!;
    public ProjectGroup Group { get; set; } = null!;
}

public class ActivityLog
{
    public int Id { get; set; }
    public int? ProjectId { get; set; }
    public int? UserId { get; set; }
    public string ActionType { get; set; } = "";
    public string Description { get; set; } = "";
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public Project? Project { get; set; }
    public User? User { get; set; }
}

public class StickyNote
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Color { get; set; } = "#fef08a";
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public User? User { get; set; }
}
