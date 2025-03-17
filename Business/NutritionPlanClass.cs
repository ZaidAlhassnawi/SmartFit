using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.DAL;

namespace Business
{
    public class clsNutritionPlan
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int PlanID { get; set; }
        public int UserID { get; set; }
        public string PlanDetails { get; set; }
        public string CreatedAt { get; set; }

        public clsNutritionPlan()
        {
            this.PlanID = -1;
            this.UserID = -1;
            this.PlanDetails = "";
            this.CreatedAt = DateTime.Now.ToString();

            Mode = enMode.AddNew;
        }

        private clsNutritionPlan(int PlanID, int UserID, string PlanDetails, string CreatedAt)
        {
            this.PlanID = PlanID;
            this.UserID = UserID;
            this.PlanDetails = PlanDetails;
            this.CreatedAt = CreatedAt;

            Mode = enMode.Update;
        }

        private bool _AddNutritionPlan()
        {
            this.PlanID = DatabaseHelper.AddNutritionPlan(this.UserID,this.PlanDetails,this.CreatedAt);

            return (this.PlanID != -1);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNutritionPlan())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;
            }

            return false;
        }

    }
}
