using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authorization
{
    public static class Permissions 
    {

        public static class Users
        {
            public const string Read = "Permissions.Users.Read";
            public const string Create = "Permissions.Users.Create";
            public const string Update = "Permissions.Users.Update";
            public const string Activate = "Permissions.Users.Activate";
            public const string Deactivate = "Permissions.Users.Deactivate";
        }
        public static class Patients 
        {
            public const string Read = "Permissions.Patients.Read";
            public const string Create = "Permissions.Patients.Create";
            public const string Update = "Permissions.Patients.Update";
            public const string Delete = "Permissions.Patients.Delete";

        }

        public static class Medicines
        {
            public const string Read = "Permissions.Medicines.Read";
            public const string Create = "Permissions.Medicines.Create";
            public const string Update = "Permissions.Medicines.Update";
          
            public const string Activate = "Permissions.Medicines.Activate";
            public const string Deactivate = "Permissions.Medicines.Deactivate";
            public const string ReadLowStock = "Permissions.Medicines.ReadLowStock";
        }

        public static class Prescriptions
        {
            public const string Read = "Permissions.Prescriptions.Read";
            public const string Create = "Permissions.Prescriptions.Create";
            public const string Update = "Permissions.Prescriptions.Update";
            public const string Delete = "Permissions.Prescriptions.Delete";
            public const string Cancel = "Permissions.Prescriptions.Cancel";
            public const string Lookup = "Permissions.Prescriptions.Lookup";
        }

        public static class Dispenses
        {
            public const string Read = "Permissions.Dispenses.Read";
            public const string Create = "Permissions.Dispenses.Create";
        }


        public static readonly string[] All = 
            [
            Users.Read,
            Users.Create,
            Users.Update,
            Users.Activate,
            Users.Deactivate,

            //Patient:
            Patients.Read,
            Patients.Create,
            Patients.Update,
            Patients.Delete,

            //Medicine:
            Medicines.Read,
            Medicines.Create,
            Medicines.Update,
            Medicines.Activate, 
            Medicines.Deactivate,
            Medicines.ReadLowStock,

            //Prescription:

           Prescriptions.Read,
           Prescriptions.Create,
           Prescriptions.Update,
           Prescriptions.Delete,
           Prescriptions.Cancel,
           Prescriptions.Lookup,

            //Dispenee:
            Dispenses.Create,
            Dispenses.Read

            ];



        }
    }

