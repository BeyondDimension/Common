namespace BD.Common.Data;

public static class IdentityTableNames
{
    const string Users = "Users";
    const string Roles = "Roles";
    const string RoleClaims = "RoleClaims";
    const string UserClaims = "UserClaims";
    const string UserLogins = "UserLogins";
    const string UserRoles = "UserRoles";
    const string UserTokens = "UserTokens";

    static void ReNameAspNetIdentity<TUser, TRole, TKey>(
        this IdentityDbContext<TUser, TRole, TKey> _,
        ModelBuilder builder,
        string users = Users,
        string roles = Roles,
        string roleClaims = RoleClaims,
        string userClaims = UserClaims,
        string userLogins = UserLogins,
        string userRoles = UserRoles,
        string userTokens = UserTokens)
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        builder.Entity<TUser>().ToTable(users).Property(x => x.PhoneNumber).HasMaxLength(SharedMaxLengths.PhoneNumber);
        builder.Entity<TRole>().ToTable(roles);
        builder.Entity<IdentityRoleClaim<TKey>>().ToTable(roleClaims);
        builder.Entity<IdentityUserClaim<TKey>>().ToTable(userClaims);
        builder.Entity<IdentityUserLogin<TKey>>().ToTable(userLogins);
        builder.Entity<IdentityUserRole<TKey>>().ToTable(userRoles);
        builder.Entity<IdentityUserToken<TKey>>().ToTable(userTokens);
    }

    public static void ReNameAspNetIdentityByClientSideDbContext<TUser, TRole, TKey>(
        this IdentityDbContext<TUser, TRole, TKey> _,
        ModelBuilder builder)
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
      => _.ReNameAspNetIdentity(builder,
          ClientSideDbContext._Users,
          ClientSideDbContext._Roles,
          ClientSideDbContext._RoleClaims,
          ClientSideDbContext._UserClaims,
          ClientSideDbContext._UserLogins,
          ClientSideDbContext._UserRoles,
          ClientSideDbContext._UserTokens);

    public static void ReNameAspNetIdentityByBackManageDbContext<TUser, TRole, TKey>(
        this IdentityDbContext<TUser, TRole, TKey> _,
        ModelBuilder builder)
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
      => _.ReNameAspNetIdentity(builder,
          BackManageDbContext._Users,
          BackManageDbContext._Roles,
          BackManageDbContext._RoleClaims,
          BackManageDbContext._UserClaims,
          BackManageDbContext._UserLogins,
          BackManageDbContext._UserRoles,
          BackManageDbContext._UserTokens);

    public static class ClientSideDbContext
    {
        public const string TablePrefix = "CS_";
        public const string _Users = TablePrefix + Users;
        public const string _Roles = TablePrefix + Roles;
        public const string _RoleClaims = TablePrefix + RoleClaims;
        public const string _UserClaims = TablePrefix + UserClaims;
        public const string _UserLogins = TablePrefix + UserLogins;
        public const string _UserRoles = TablePrefix + UserRoles;
        public const string _UserTokens = TablePrefix + UserTokens;
    }

    public static class BackManageDbContext
    {
        public const string TablePrefix = "BM_";
        public const string _Users = TablePrefix + Users;
        public const string _Roles = TablePrefix + Roles;
        public const string _RoleClaims = TablePrefix + RoleClaims;
        public const string _UserClaims = TablePrefix + UserClaims;
        public const string _UserLogins = TablePrefix + UserLogins;
        public const string _UserRoles = TablePrefix + UserRoles;
        public const string _UserTokens = TablePrefix + UserTokens;
    }
}