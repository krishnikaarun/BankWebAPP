using System;
using System.Xml.Schema;
using BankAPPWeb.Model;
using Microsoft.Data.Sqlite;

namespace BankAPPWeb.accountDAO
{
    public class AccountDAO
    {
        private readonly SqliteConnection conn;
        public AccountDAO()
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "./BankAPP_Data.db";
            connectionStringBuilder.Mode = SqliteOpenMode.ReadWriteCreate;
            conn = new SqliteConnection(connectionStringBuilder.ConnectionString);
        }
        public User Login(int UserID, int PIN)
        {
            string selectloginQuery = " SELECT  UserID, UserName  FROM Customers where UserID = " + UserID + " AND PIN = " + PIN;
            SqliteCommand view = new SqliteCommand(selectloginQuery, conn);
            conn.Open();
            SqliteDataReader dr = view.ExecuteReader();
            User user1 = new User();
            try
             {
                while (dr.Read())
                {
                    user1.UserID = dr.GetInt32(0);
                }

                return user1;
            }
             catch (Exception e)
             {
                 Console.WriteLine(e);
                 user1.UserID = 0;
                 return user1;
             }
             finally
             {
                 conn.Close();
             }
        }
        public int BalanceCheck(String UserID)
        {
            conn.Open();
            string BalanceCheckQuery = " SELECT TotAmount FROM Bank where UserID = " + UserID;
            SqliteCommand view = new SqliteCommand(BalanceCheckQuery, conn);
            SqliteDataReader reader = view.ExecuteReader();
            int[] Balance = new int[1];
            while (reader.Read())
            {
                Balance[0] = reader.GetInt32(0);
            }
            conn.Close();
            return Balance[0];
        }
        public int Deposit(int DepositAmount,string value)
        {
            string selectBankQuery = "SELECT TotAmount FROM Bank WHERE UserID =" + value;
            SqliteCommand view = new SqliteCommand(selectBankQuery, conn);
            conn.Open();
            SqliteDataReader reader = view.ExecuteReader();
            if (reader.Read())
            {
                int[] TotAmount = new int[1];
                TotAmount[0] = reader.GetInt32(0);
                TotAmount[0] += DepositAmount;
                conn.Close();
                UpdateAmount(Convert.ToInt32(value), TotAmount[0]);
                InsertDepositTrans(Convert.ToInt32(value), DepositAmount);
            }
            return DepositAmount;
        }
        public int PINChange(int UserID, int NewPIN)
        {
            string NewPINChangeQuery = "UPDATE  Customers SET Pin=" + NewPIN + " where UserID = " + UserID;
            SqliteCommand updateCommand = new SqliteCommand(NewPINChangeQuery, conn);
            conn.Open();
            updateCommand.ExecuteReader();
            conn.Close();
            return NewPIN;
        }
        public int Withdraw(int WithdrawAmount,int UID)
        {
            string selectBankQuery = "SELECT TotAmount FROM Bank WHERE UserID =" + UID;
            SqliteCommand view = new SqliteCommand(selectBankQuery, conn);
            conn.Open();
            SqliteDataReader reader = view.ExecuteReader();
            if (reader.Read())
            {
                int[] TotAmount = new int[1];
                TotAmount[0] = reader.GetInt32(0);
                conn.Close();
                if (WithdrawAmount <= TotAmount[0])
                {
                    TotAmount[0] -= WithdrawAmount;
                    Console.WriteLine(TotAmount[0]);
                    UpdateAmount(UID, TotAmount[0]);
                    InsertWithdrawTrans(UID,WithdrawAmount);
                }
            }
            return WithdrawAmount;
        }
        public int Transfer(int Amt,int UID,int UID2)
        {
            string selectBankQuery = "SELECT TotAmount FROM Bank WHERE UserID =" + UID;
            SqliteCommand view = new SqliteCommand(selectBankQuery, conn);
            conn.Open();
            SqliteDataReader reader = view.ExecuteReader();
            reader.Read();
            int TotAmount1 = reader.GetInt32(0);
            conn.Close();
            //
            string selectBankQuery2 = "SELECT TotAmount FROM Bank WHERE UserID =" + UID2;
            SqliteCommand view2 = new SqliteCommand(selectBankQuery, conn);
            conn.Open();
            SqliteDataReader reader2 = view2.ExecuteReader();
            reader2.Read();
            int TotAmount2 = reader2.GetInt32(0);
            conn.Close();
            //
            string selectBankQuery3 = "SELECT AccountNo FROM Bank WHERE UserID =" + UID;
            SqliteCommand view3 = new SqliteCommand(selectBankQuery, conn);
            conn.Open();
            SqliteDataReader reader3 = view3.ExecuteReader();
            reader3.Read();
            int AccountNo1 = reader3.GetInt32(0);
            conn.Close();
            //
            string selectBankQuery4 = "SELECT AccountNo FROM Bank WHERE UserID =" + UID2;
            SqliteCommand view4 = new SqliteCommand(selectBankQuery, conn);
            conn.Open();
            SqliteDataReader reader4 = view4.ExecuteReader();
            reader4.Read();
            int AccountNo2 = reader4.GetInt32(0);
            conn.Close();
            TotAmount1 -= Amt;
            TotAmount2 += Amt;
            UpdateAmount(UID, TotAmount1);
            UpdateAmount(UID2, TotAmount2);
            int bal = BalanceCheck(Convert.ToString(UID));
            string InsertTransQuery = "INSERT INTO Trans (CD,Amount,AccountNo1,AccountNo2,UserID,Dated,bal) VALUES ('T2'," + TotAmount1 + "," + AccountNo1 + "," + AccountNo2 + "," + UID + ",@DATE," + bal + ")";
        //  string InsertTransQuery = "INSERT INTO Trans(CD, Amount, AccountNo1, AccountNo2, UserID, Dated, bal) VALUES('T2', 500, 430004, 430001, 1001, '2020-05-12 00:00:00.000', 0);";
            SqliteCommand updateCommand = new SqliteCommand(InsertTransQuery, conn);
            updateCommand.Parameters.AddWithValue("@DATE", DateTime.Now);
            conn.Open();
            updateCommand.ExecuteNonQuery();
            conn.Close();
            //
            int bal2 = BalanceCheck(Convert.ToString(UID));
            string InsertTransQuery2 = "INSERT INTO Trans (CD,Amount,AccountNo1,AccountNo2,UserID,Dated,bal) VALUES ('T4'," + TotAmount2 + "," + AccountNo2 + "," + AccountNo1+ "," + UID2 + ",@DATE," + bal2 + ")";
            SqliteCommand updateCommand2 = new SqliteCommand(InsertTransQuery2, conn);
            updateCommand2.Parameters.AddWithValue("@DATE", DateTime.Now);
            conn.Open();
            updateCommand2.ExecuteNonQuery();
            conn.Close();
            /*SqliteCommand updateCommand2 = new SqliteCommand(InsertTransQuery2, conn);
            updateCommand2.Parameters.AddWithValue("@DATE", DateTime.Now);
            conn.Open();
            updateCommand2.ExecuteNonQuery();
            conn.Close();*/
            return Amt;
        }
        public void UpdateAmount(int UserID, int TotAmount)
        {
            string UpdateQuery = "UPDATE  Bank SET TotAmount =" + TotAmount + " where UserID = " + UserID;
            SqliteCommand updateCommand = new SqliteCommand(UpdateQuery, conn);
            conn.Open();
            updateCommand.ExecuteNonQuery();
            conn.Close();
        }
        public void InsertDepositTrans(int UserID, int TotAmount)
        {
            string selectTransQuery = "SELECT AccountNo1 from Trans Where UserID=" + UserID;
            SqliteCommand view = new SqliteCommand(selectTransQuery, conn);
            conn.Open();
            SqliteDataReader reader = view.ExecuteReader();
            int[] AccountNo = new int[1];
            if (reader.Read())
            {
                AccountNo[0] = reader.GetInt32(0);
            }
            conn.Close();
            int bal = BalanceCheck(Convert.ToString(UserID));
            string InsertTransQuery = "INSERT INTO Trans (CD,Amount,AccountNo1,AccountNo2,UserID,Dated,bal) VALUES ('C'," + TotAmount + ","+AccountNo[0]+"," + AccountNo[0] + "," + UserID + ",@DATE,"+bal+")";
            SqliteCommand updateCommand = new SqliteCommand(InsertTransQuery, conn);
            updateCommand.Parameters.AddWithValue("@DATE", DateTime.Now);
            conn.Open();
            updateCommand.ExecuteNonQuery();
            conn.Close();
        }
        public void InsertWithdrawTrans(int UserID, int TotAmount)
        {
            string selectTransQuery = "SELECT AccountNo1 from Trans Where UserID=" + UserID;
            SqliteCommand view = new SqliteCommand(selectTransQuery, conn);
            conn.Open();
            SqliteDataReader reader = view.ExecuteReader();
            int[] AccountNo = new int[1];
            if (reader.Read())
            {
                AccountNo[0] = reader.GetInt32(0);
            }
            conn.Close();
            int bal = BalanceCheck(Convert.ToString(UserID));
            bal -= TotAmount;
            string InsertTransQuery = "INSERT INTO Trans (CD,Amount,AccountNo1,AccountNo2,UserID,Dated,bal) VALUES ('D'," + TotAmount + "," +AccountNo[0]+","+ AccountNo[0] + "," + UserID + ",@DATE,"+bal+")";
            SqliteCommand updateCommand = new SqliteCommand(InsertTransQuery, conn);
            updateCommand.Parameters.AddWithValue("@DATE", DateTime.Now);
            conn.Open();
            updateCommand.ExecuteNonQuery();
            conn.Close();
        }
        public User[] TransLog(int UserID)
        {
            int i = 0;
            string countTransQuery = "SELECT COUNT(*) FROM Trans WHERE UserID = " + UserID;
            SqliteCommand countCommand = new SqliteCommand(countTransQuery, conn);
            conn.Open();
            Int64 n = (Int64)countCommand.ExecuteScalar();
            User[] Tran1 = new User[n];
            string TransLogQuery = "SELECT TransID, CD, Amount, AccountNo1,AccountNo2, Dated, bal FROM Trans WHERE UserID = " + UserID;
            //string TransLogQuery = "select Trans.TransID, Bank.TotAmount, Trans.Amount, Trans.CD ,Trans.Dated FROM Trans INNER JOIN Bank ON Bank.UserID = Trans.UserID WHERE Bank.UserID =" + UserID;
            SqliteCommand selectCommand = new SqliteCommand(TransLogQuery, conn);
            SqliteDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                User Tran = new User();
                Tran.TransID = reader.GetInt32(0);
                Tran.CD = reader.GetString(1);
                Tran.Amount = reader.GetInt32(2);
                Tran.AccountNo1 = reader.GetInt32(3);
                Tran.AccountNo2 = reader.GetInt32(4);
                Tran.Dated = reader.GetString(5);
                Tran.bal = reader.GetInt32(6);
                //Tran.TotAmount = reader.GetInt32(1);
                Tran1[i] = Tran;
                i++;
            }
            conn.Close();
            return Tran1;
        }
    }
}