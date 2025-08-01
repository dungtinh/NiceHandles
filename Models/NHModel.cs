using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace NiceHandles.Models
{
    public partial class NHModel : DbContext
    {
        public NHModel()
            : base("name=NHModel")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Attendance> Attendances { get; set; }
        public virtual DbSet<BangChamCong> BangChamCongs { get; set; }
        public virtual DbSet<CanBo> CanBoes { get; set; }
        public virtual DbSet<CanBoDiaPhuong> CanBoDiaPhuongs { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<CongViec> CongViecs { get; set; }
        public virtual DbSet<CoQuanGiaiQuyet> CoQuanGiaiQuyets { get; set; }
        public virtual DbSet<CuocHop> CuocHops { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<ChamCong> ChamCongs { get; set; }
        public virtual DbSet<ChatFile> ChatFiles { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<ChatSession> ChatSessions { get; set; }
        public virtual DbSet<chotso> chotsoes { get; set; }
        public virtual DbSet<di1cua> di1cua { get; set; }
        public virtual DbSet<Diary> Diaries { get; set; }
        public virtual DbSet<didiachinh> didiachinhs { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<donthu> donthus { get; set; }
        public virtual DbSet<Dove> Doves { get; set; }
        public virtual DbSet<Dove_ThuChi> Dove_ThuChi { get; set; }
        public virtual DbSet<dove_wf> dove_wf { get; set; }
        public virtual DbSet<dovelichsu> dovelichsus { get; set; }
        public virtual DbSet<DSChoDuyet> DSChoDuyets { get; set; }
        public virtual DbSet<DuChi> DuChis { get; set; }
        public virtual DbSet<DuChiNhatKy> DuChiNhatKies { get; set; }
        public virtual DbSet<DuChiNhatKyWF> DuChiNhatKyWFs { get; set; }
        public virtual DbSet<DvhcC1> DvhcC1 { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<fk_CanBo_Address> fk_CanBo_Address { get; set; }
        public virtual DbSet<fk_contract_service> fk_contract_service { get; set; }
        public virtual DbSet<fk_congviec_hoso> fk_congviec_hoso { get; set; }
        public virtual DbSet<fk_congviec_service> fk_congviec_service { get; set; }
        public virtual DbSet<fk_chotso_thuchi> fk_chotso_thuchi { get; set; }
        public virtual DbSet<fk_service_document> fk_service_document { get; set; }
        public virtual DbSet<GroupField> GroupFields { get; set; }
        public virtual DbSet<GroupMember> GroupMembers { get; set; }
        public virtual DbSet<GroupMember_Account> GroupMember_Account { get; set; }
        public virtual DbSet<GiaoNhanGiayTo> GiaoNhanGiayToes { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<HoSo> HoSoes { get; set; }
        public virtual DbSet<Infomation> Infomations { get; set; }
        public virtual DbSet<InOut> InOuts { get; set; }
        public virtual DbSet<InOutChoDuyet> InOutChoDuyets { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<KPI> KPIs { get; set; }
        public virtual DbSet<KPIChiTieu> KPIChiTieux { get; set; }
        public virtual DbSet<KPILink> KPILinks { get; set; }
        public virtual DbSet<LanDau> LanDaus { get; set; }
        public virtual DbSet<landau_quytrinh> landau_quytrinh { get; set; }
        public virtual DbSet<Money> Moneys { get; set; }
        public virtual DbSet<Notice> Notices { get; set; }
        public virtual DbSet<NguoiHop> NguoiHops { get; set; }
        public virtual DbSet<nhatkydonthu> nhatkydonthus { get; set; }
        public virtual DbSet<NhiemVu> NhiemVus { get; set; }
        public virtual DbSet<OneDoorFile> OneDoorFiles { get; set; }
        public virtual DbSet<Page4> Page4 { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<PayPlan> PayPlans { get; set; }
        public virtual DbSet<PriceByArea> PriceByAreas { get; set; }
        public virtual DbSet<Progress> Progresses { get; set; }
        public virtual DbSet<Progress_Document> Progress_Document { get; set; }
        public virtual DbSet<progress_file> progress_file { get; set; }
        public virtual DbSet<progress_file_option> progress_file_option { get; set; }
        public virtual DbSet<PhuCap> PhuCaps { get; set; }
        public virtual DbSet<QuanTriVanHanh> QuanTriVanHanhs { get; set; }
        public virtual DbSet<QuyDove> QuyDoves { get; set; }
        public virtual DbSet<Reporter> Reporters { get; set; }
        public virtual DbSet<Reward> Rewards { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<service_step_day> service_step_day { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<Social> Socials { get; set; }
        public virtual DbSet<SoHoKhau> SoHoKhaus { get; set; }
        public virtual DbSet<Step> Steps { get; set; }
        public virtual DbSet<StepAccount> StepAccounts { get; set; }
        public virtual DbSet<Stocker> Stockers { get; set; }
        public virtual DbSet<syslog> syslogs { get; set; }
        public virtual DbSet<TachHop> TachHops { get; set; }
        public virtual DbSet<TachThua> TachThuas { get; set; }
        public virtual DbSet<TaiLieu> TaiLieux { get; set; }
        public virtual DbSet<task> tasks { get; set; }
        public virtual DbSet<ThongTinCaNhan> ThongTinCaNhans { get; set; }
        public virtual DbSet<ThuaDat> ThuaDats { get; set; }
        public virtual DbSet<thuchi> thuchis { get; set; }
        public virtual DbSet<TreoPhuon> TreoPhuons { get; set; }
        public virtual DbSet<TruLuong> TruLuongs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Val> Vals { get; set; }
        public virtual DbSet<ViecPhaiLam> ViecPhaiLams { get; set; }
        public virtual DbSet<wf_contract> wf_contract { get; set; }
        public virtual DbSet<wf_hoso> wf_hoso { get; set; }
        public virtual DbSet<wf_hoso_step> wf_hoso_step { get; set; }
        public virtual DbSet<wf_inout> wf_inout { get; set; }
        public virtual DbSet<wf_quydove> wf_quydove { get; set; }
        public virtual DbSet<Hoso_Step> Hoso_Step { get; set; }
        public virtual DbSet<DSBanLamViec> DSBanLamViecs { get; set; }
        public virtual DbSet<DSHoSo> DSHoSoes { get; set; }
        public virtual DbSet<DSMotCua> DSMotCuas { get; set; }
        public virtual DbSet<PartnerAccount> PartnerAccounts { get; set; }
        public virtual DbSet<vCalculatorMoney> vCalculatorMoneys { get; set; }
        public virtual DbSet<vCanBoDiaPhuongC1> vCanBoDiaPhuongC1 { get; set; }
        public virtual DbSet<vCanBoDiaPhuongC2> vCanBoDiaPhuongC2 { get; set; }
        public virtual DbSet<vContract> vContracts { get; set; }
        public virtual DbSet<vContractHardFile> vContractHardFiles { get; set; }
        public virtual DbSet<vContractSelect> vContractSelects { get; set; }
        public virtual DbSet<vChartInOut> vChartInOuts { get; set; }
        public virtual DbSet<vDi1Cua> vDi1Cua { get; set; }
        public virtual DbSet<vDuChi> vDuChis { get; set; }
        public virtual DbSet<vDvhcC1> vDvhcC1 { get; set; }
        public virtual DbSet<vHomeWFInOut> vHomeWFInOuts { get; set; }
        public virtual DbSet<vHoSo> vHoSoes { get; set; }
        public virtual DbSet<vInOut> vInOuts { get; set; }
        public virtual DbSet<vMyFile> vMyFiles { get; set; }
        public virtual DbSet<vMyReward> vMyRewards { get; set; }
        public virtual DbSet<vPayPlan> vPayPlans { get; set; }
        public virtual DbSet<vPending> vPendings { get; set; }
        public virtual DbSet<vPhongDoVe> vPhongDoVes { get; set; }
        public virtual DbSet<vSettingGroupMember> vSettingGroupMembers { get; set; }
        public virtual DbSet<vSoanThao> vSoanThaos { get; set; }
        public virtual DbSet<vStorage> vStorages { get; set; }
        public virtual DbSet<vTienDo> vTienDoes { get; set; }
        public virtual DbSet<vTheoDoiChi> vTheoDoiChis { get; set; }
        public virtual DbSet<vTheoDoiTienDo> vTheoDoiTienDoes { get; set; }
        public virtual DbSet<vViecPhaiLam> vViecPhaiLams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.bankno)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.phoneno)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.sogiayto)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<CanBo>()
                .Property(e => e.phone)
                .IsUnicode(false);

            modelBuilder.Entity<ChatSession>()
                .HasMany(e => e.ChatFiles)
                .WithRequired(e => e.ChatSession)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChatSession>()
                .HasMany(e => e.ChatMessages)
                .WithRequired(e => e.ChatSession)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Document>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<DvhcC1>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<Field>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Infomation>()
                .Property(e => e.a_sogiayto)
                .IsUnicode(false);

            modelBuilder.Entity<Infomation>()
                .Property(e => e.a_masothue)
                .IsUnicode(false);

            modelBuilder.Entity<Infomation>()
                .Property(e => e.a_sogiayto1)
                .IsUnicode(false);

            modelBuilder.Entity<Infomation>()
                .Property(e => e.b_tobando)
                .IsUnicode(false);

            modelBuilder.Entity<Infomation>()
                .Property(e => e.d_sogiayto)
                .IsUnicode(false);

            modelBuilder.Entity<Infomation>()
                .Property(e => e.e_sogiayto)
                .IsUnicode(false);

            modelBuilder.Entity<Infomation>()
                .Property(e => e.e_masothue)
                .IsUnicode(false);

            modelBuilder.Entity<InOut>()
                .Property(e => e.code)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Money>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<Partner>()
                .Property(e => e.id_no)
                .IsUnicode(false);

            modelBuilder.Entity<Partner>()
                .Property(e => e.id_type)
                .IsUnicode(false);

            modelBuilder.Entity<Service>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<Setting>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<Step>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<TachHop>()
                .Property(e => e.a_sogiayto)
                .IsUnicode(false);

            modelBuilder.Entity<TachHop>()
                .Property(e => e.a_masothue)
                .IsUnicode(false);

            modelBuilder.Entity<TachHop>()
                .Property(e => e.a_sogiayto1)
                .IsUnicode(false);

            modelBuilder.Entity<TachHop>()
                .Property(e => e.b_sogiaychungnhan)
                .IsUnicode(false);

            modelBuilder.Entity<TachHop>()
                .Property(e => e.b_tobando)
                .IsUnicode(false);

            modelBuilder.Entity<TachHop>()
                .Property(e => e.d_sogiayto)
                .IsUnicode(false);

            modelBuilder.Entity<TachHop>()
                .Property(e => e.e_sogiayto)
                .IsUnicode(false);

            modelBuilder.Entity<TachHop>()
                .Property(e => e.e_masothue)
                .IsUnicode(false);

            modelBuilder.Entity<TachThua>()
                .Property(e => e.a_sogiayto)
                .IsUnicode(false);

            modelBuilder.Entity<TachThua>()
                .Property(e => e.a_masothue)
                .IsUnicode(false);

            modelBuilder.Entity<TachThua>()
                .Property(e => e.a_sogiayto1)
                .IsUnicode(false);

            modelBuilder.Entity<TachThua>()
                .Property(e => e.b_sogiaychungnhan)
                .IsUnicode(false);

            modelBuilder.Entity<TachThua>()
                .Property(e => e.b_tobando)
                .IsUnicode(false);

            modelBuilder.Entity<TachThua>()
                .Property(e => e.d_sogiayto)
                .IsUnicode(false);

            modelBuilder.Entity<TachThua>()
                .Property(e => e.e_sogiayto)
                .IsUnicode(false);

            modelBuilder.Entity<TachThua>()
                .Property(e => e.e_masothue)
                .IsUnicode(false);

            modelBuilder.Entity<ThongTinCaNhan>()
                .Property(e => e.sogiayto)
                .IsUnicode(false);

            modelBuilder.Entity<ThongTinCaNhan>()
                .Property(e => e.masothue)
                .IsUnicode(false);

            modelBuilder.Entity<ThongTinCaNhan>()
                .Property(e => e.sogiayto1)
                .IsUnicode(false);

            modelBuilder.Entity<ThongTinCaNhan>()
                .Property(e => e.masothue1)
                .IsUnicode(false);

            modelBuilder.Entity<ThuaDat>()
                .Property(e => e.sogiaychungnhan)
                .IsUnicode(false);

            modelBuilder.Entity<ThuaDat>()
                .Property(e => e.sothua)
                .IsUnicode(false);

            modelBuilder.Entity<ThuaDat>()
                .Property(e => e.tobando)
                .IsUnicode(false);

            modelBuilder.Entity<ThuaDat>()
                .Property(e => e.dientich)
                .IsUnicode(false);

            modelBuilder.Entity<DSBanLamViec>()
                .Property(e => e.step_code)
                .IsUnicode(false);

            modelBuilder.Entity<PartnerAccount>()
                .Property(e => e.id_no)
                .IsUnicode(false);

            modelBuilder.Entity<PartnerAccount>()
                .Property(e => e.id_type)
                .IsUnicode(false);

            modelBuilder.Entity<vContract>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<vDvhcC1>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<vPhongDoVe>()
                .Property(e => e.b_tobando)
                .IsUnicode(false);

            modelBuilder.Entity<vSoanThao>()
                .Property(e => e.step_code)
                .IsUnicode(false);

            modelBuilder.Entity<vTienDo>()
                .Property(e => e.step_code)
                .IsUnicode(false);
        }
    }
}
