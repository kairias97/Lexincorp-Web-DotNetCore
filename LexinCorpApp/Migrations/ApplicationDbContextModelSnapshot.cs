﻿// <auto-generated />
using System;
using LexincorpApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LexincorpApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LexincorpApp.Models.Activity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ActivityType");

                    b.Property<decimal>("BillableQuantity");

                    b.Property<decimal>("BillableRate");

                    b.Property<int?>("BillableRetainerId");

                    b.Property<int>("ClientId");

                    b.Property<int>("CreatorId");

                    b.Property<string>("Description");

                    b.Property<decimal>("HoursWorked");

                    b.Property<bool>("IsBilled");

                    b.Property<int?>("ItemId");

                    b.Property<int?>("PackageId");

                    b.Property<bool>("PayTaxes");

                    b.Property<DateTime>("RealizationDate");

                    b.Property<int>("ServiceId");

                    b.Property<decimal>("Subtotal");

                    b.Property<decimal>("TaxesAmount");

                    b.Property<decimal>("TotalAmount");

                    b.HasKey("Id");

                    b.HasIndex("BillableRetainerId");

                    b.HasIndex("ClientId");

                    b.HasIndex("CreatorId");

                    b.HasIndex("ItemId");

                    b.HasIndex("PackageId");

                    b.HasIndex("ServiceId");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("LexincorpApp.Models.ActivityExpense", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ActivityId");

                    b.Property<int>("ExpenseId");

                    b.Property<int>("Quantity");

                    b.Property<DateTime>("RealizationDate");

                    b.Property<decimal>("TotalAmount");

                    b.Property<decimal>("UnitAmount");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("ExpenseId");

                    b.ToTable("ActivityExpenses");
                });

            modelBuilder.Entity("LexincorpApp.Models.Attorney", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<DateTime>("AdmissionDate");

                    b.Property<string>("AssignedPhoneNumber")
                        .IsRequired();

                    b.Property<int>("DepartmentId");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("EmergencyContact")
                        .IsRequired();

                    b.Property<string>("EmergencyContactPhoneNumber")
                        .IsRequired();

                    b.Property<string>("IdentificationNumber")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("NotaryCode")
                        .IsRequired();

                    b.Property<string>("PersonalPhoneNumber")
                        .IsRequired();

                    b.Property<int>("UserId");

                    b.Property<decimal>("VacationCount")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0m);

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("NotaryCode")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Attorneys");
                });

            modelBuilder.Entity("LexincorpApp.Models.BillableRetainer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("AdditionalFeePerHour");

                    b.Property<decimal>("AgreedFee");

                    b.Property<decimal>("AgreedHours");

                    b.Property<string>("BillingDescription");

                    b.Property<int>("ClientId");

                    b.Property<decimal>("ConsumedHours")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0m);

                    b.Property<int>("CreatorId");

                    b.Property<bool>("IsBilled");

                    b.Property<int>("Month");

                    b.Property<string>("Name");

                    b.Property<int>("RetainerId");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("CreatorId");

                    b.HasIndex("RetainerId");

                    b.ToTable("BillableRetainers");
                });

            modelBuilder.Entity("LexincorpApp.Models.BillDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ActivityId");

                    b.Property<int>("BillDetailType");

                    b.Property<int>("BillHeaderId");

                    b.Property<string>("Description");

                    b.Property<decimal?>("FixedAmount");

                    b.Property<decimal?>("Quantity");

                    b.Property<decimal>("Subtotal");

                    b.Property<decimal>("TaxesAmount");

                    b.Property<decimal?>("UnitRate");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("BillHeaderId");

                    b.ToTable("BillDetails");
                });

            modelBuilder.Entity("LexincorpApp.Models.BillHeader", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("BillDate");

                    b.Property<decimal?>("BillDiscount");

                    b.Property<int?>("BillDiscountType");

                    b.Property<int>("BillMonth");

                    b.Property<string>("BillName");

                    b.Property<decimal>("BillSubtotal");

                    b.Property<int>("BillYear");

                    b.Property<int>("ClientId");

                    b.Property<decimal>("Taxes");

                    b.Property<decimal>("Total");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("BillHeaders");
                });

            modelBuilder.Entity("LexincorpApp.Models.BillingMode", b =>
                {
                    b.Property<int>("BillingModeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("BillingModeId");

                    b.ToTable("BillingModes");
                });

            modelBuilder.Entity("LexincorpApp.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("LexincorpApp.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<string>("Address");

                    b.Property<bool>("BillingInEnglish");

                    b.Property<int>("BillingModeId");

                    b.Property<string>("CellPhoneNumber");

                    b.Property<int>("ClientTypeId");

                    b.Property<string>("Contact")
                        .IsRequired();

                    b.Property<string>("ContactEmail");

                    b.Property<string>("ContactJobName");

                    b.Property<string>("ContactPhone");

                    b.Property<int>("DocumentDeliveryMethodId");

                    b.Property<string>("Email");

                    b.Property<string>("Fax");

                    b.Property<decimal?>("FixedCostPerHour");

                    b.Property<bool>("IsInternational");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<bool>("PayTaxes");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("ReferredBy");

                    b.Property<string>("TributaryId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAlternateKey("TributaryId")
                        .HasName("UQ_Client_TributaryId");

                    b.HasIndex("BillingModeId");

                    b.HasIndex("ClientTypeId");

                    b.HasIndex("DocumentDeliveryMethodId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("LexincorpApp.Models.ClientType", b =>
                {
                    b.Property<int>("ClientTypeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("ClientTypeId");

                    b.ToTable("ClientTypes");
                });

            modelBuilder.Entity("LexincorpApp.Models.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("DepartmentId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("LexincorpApp.Models.DocumentDeliveryMethod", b =>
                {
                    b.Property<int>("DocumentDeliveryMethodId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("DocumentDeliveryMethodId");

                    b.ToTable("DocumentDeliveryMethods");
                });

            modelBuilder.Entity("LexincorpApp.Models.Expense", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<decimal>("Amount");

                    b.Property<string>("EnglishDescription")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("SpanishDescription")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("LexincorpApp.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<decimal>("Amount");

                    b.Property<string>("EnglishDescription")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("SpanishDescription")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("LexincorpApp.Models.Package", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount");

                    b.Property<int>("ClientId");

                    b.Property<int?>("CreatorId");

                    b.Property<int>("CreatorUserId");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<bool>("IsBilled")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<bool>("IsFinished")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime?>("RealizationDate")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("CreatorId");

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("LexincorpApp.Models.Retainer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<string>("EnglishName");

                    b.Property<string>("SpanishName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Retainers");
                });

            modelBuilder.Entity("LexincorpApp.Models.RetainerSubscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("AdditionalFeePerHour");

                    b.Property<decimal>("AgreedFee");

                    b.Property<decimal>("AgreedHours");

                    b.Property<int>("ClientId");

                    b.Property<int>("CreatorId");

                    b.Property<int>("RetainerId");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("CreatorId");

                    b.HasIndex("RetainerId");

                    b.ToTable("RetainerSubscriptions");
                });

            modelBuilder.Entity("LexincorpApp.Models.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<int>("CategoryId");

                    b.Property<string>("EnglishDescription")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("SpanishDescription")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("LexincorpApp.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("Password");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAlternateKey("Username")
                        .HasName("UQ_User_Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("LexincorpApp.Models.VacationsMovement", b =>
                {
                    b.Property<int>("VacationsMovementId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AttorneyId");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<int>("MovementType");

                    b.Property<decimal>("Quantity");

                    b.Property<string>("Reason")
                        .IsRequired();

                    b.HasKey("VacationsMovementId");

                    b.HasIndex("AttorneyId");

                    b.ToTable("VacationsMovements");
                });

            modelBuilder.Entity("LexincorpApp.Models.VacationsRequest", b =>
                {
                    b.Property<int>("VacationsRequestId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AttorneyId");

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<bool?>("IsApproved");

                    b.Property<decimal>("Quantity");

                    b.Property<string>("Reason")
                        .IsRequired();

                    b.Property<DateTime>("StartDate");

                    b.HasKey("VacationsRequestId");

                    b.HasIndex("AttorneyId");

                    b.ToTable("VacationsRequests");
                });

            modelBuilder.Entity("LexincorpApp.Models.Activity", b =>
                {
                    b.HasOne("LexincorpApp.Models.BillableRetainer", "BillableRetainer")
                        .WithMany()
                        .HasForeignKey("BillableRetainerId");

                    b.HasOne("LexincorpApp.Models.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LexincorpApp.Models.User", "Creator")
                        .WithMany("Activities")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LexincorpApp.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId");

                    b.HasOne("LexincorpApp.Models.Package", "Package")
                        .WithMany()
                        .HasForeignKey("PackageId");

                    b.HasOne("LexincorpApp.Models.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LexincorpApp.Models.ActivityExpense", b =>
                {
                    b.HasOne("LexincorpApp.Models.Activity", "Activity")
                        .WithMany("ActivityExpenses")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LexincorpApp.Models.Expense", "Expense")
                        .WithMany()
                        .HasForeignKey("ExpenseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LexincorpApp.Models.Attorney", b =>
                {
                    b.HasOne("LexincorpApp.Models.Department", "Department")
                        .WithMany("Attorneys")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LexincorpApp.Models.User", "User")
                        .WithOne("Attorney")
                        .HasForeignKey("LexincorpApp.Models.Attorney", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LexincorpApp.Models.BillableRetainer", b =>
                {
                    b.HasOne("LexincorpApp.Models.Client", "Client")
                        .WithMany("BillableRetainers")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LexincorpApp.Models.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LexincorpApp.Models.Retainer", "Retainer")
                        .WithMany("BillableRetainers")
                        .HasForeignKey("RetainerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LexincorpApp.Models.BillDetail", b =>
                {
                    b.HasOne("LexincorpApp.Models.Activity", "Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId");

                    b.HasOne("LexincorpApp.Models.BillHeader", "BillHeader")
                        .WithMany("BillDetails")
                        .HasForeignKey("BillHeaderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LexincorpApp.Models.BillHeader", b =>
                {
                    b.HasOne("LexincorpApp.Models.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LexincorpApp.Models.Client", b =>
                {
                    b.HasOne("LexincorpApp.Models.BillingMode", "BillingMode")
                        .WithMany()
                        .HasForeignKey("BillingModeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LexincorpApp.Models.ClientType", "ClientType")
                        .WithMany("Clients")
                        .HasForeignKey("ClientTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LexincorpApp.Models.DocumentDeliveryMethod", "DocumentDeliveryMethod")
                        .WithMany()
                        .HasForeignKey("DocumentDeliveryMethodId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LexincorpApp.Models.Package", b =>
                {
                    b.HasOne("LexincorpApp.Models.Client", "Client")
                        .WithMany("Packages")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LexincorpApp.Models.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");
                });

            modelBuilder.Entity("LexincorpApp.Models.RetainerSubscription", b =>
                {
                    b.HasOne("LexincorpApp.Models.Client", "Client")
                        .WithMany("RetainerSubscriptions")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LexincorpApp.Models.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LexincorpApp.Models.Retainer", "Retainer")
                        .WithMany("RetainerSubscriptions")
                        .HasForeignKey("RetainerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LexincorpApp.Models.Service", b =>
                {
                    b.HasOne("LexincorpApp.Models.Category", "Category")
                        .WithMany("Services")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LexincorpApp.Models.VacationsMovement", b =>
                {
                    b.HasOne("LexincorpApp.Models.Attorney", "Attorney")
                        .WithMany()
                        .HasForeignKey("AttorneyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LexincorpApp.Models.VacationsRequest", b =>
                {
                    b.HasOne("LexincorpApp.Models.Attorney", "Attorney")
                        .WithMany()
                        .HasForeignKey("AttorneyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
