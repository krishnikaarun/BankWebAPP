using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;
using BankAPPWeb.accountDAO;
using BankAPPWeb.Model;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace BankAPPWeb.Banks
{
    public class Bank
    {
        public int UserID = 0, PIN = 0, UserName = 0, UserID2 = 0, ToAmount = 0;
        AccountDAO accountdao;
        public Bank()
        {
            this.accountdao = new AccountDAO();
        }
        public User LoginUser(int UserID, int PIN)
        {
            User User1 = this.accountdao.Login(UserID, PIN);
            return User1;
        }
        public User BalanceCheckUser(int UserID)
        {
            User User2 = this.accountdao.BalanceCheck(UserID);
            return User2;
        }
            //main operation funtion
            public void MainAtmn(int UserID , int PIN)
            {                     
                User User1 = this.accountdao.Login(UserID, PIN);
                if (User1.UserID != 0)
                {
                    Console.Clear();
                    Console.WriteLine("Welcome {0}!", User1.UserName);
                    int op = Convert.ToInt32(Console.ReadLine());

                    switch (op)
                    {
                        case 1:
                            try
                            {
                                int DepositAmount = 0;
                                Console.Write("Enter the amount to Deposit: ");
                                DepositAmount = Convert.ToInt32(Console.ReadLine());
                                this.accountdao.Deposit(UserID, DepositAmount);
                                Console.Beep();
                                Console.WriteLine("Deposit is Successful !");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error in Deposit !: " + e.Message);
                            }
                            break;
                        case 2:
                            try
                            {
                                int WithdrawAmount = 0;
                                Console.Write("Enter the amount to withdraw: ");
                                WithdrawAmount = Convert.ToInt32(Console.ReadLine());
                                if (WithdrawAmount > 0)
                                {
                                    this.accountdao.Withdraw(UserID, WithdrawAmount);
                                    Console.Beep();
                                    Console.WriteLine("Withdraw is Successful !");

                                }
                                else
                                {
                                    Console.WriteLine("Enter a large Amount !");
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error in Withdraw !: " + e.Message);
                            }
                            break;
                        case 3:
                            try
                            {
                                Console.Write("Enter the UserID to Transfer:");
                                UserID2 = Convert.ToInt32(Console.ReadLine());
                                Console.Write("Enter the Amount to transfer :");
                                ToAmount = Convert.ToInt32(Console.ReadLine());
                                this.accountdao.Withdraw(UserID, ToAmount);
                                this.accountdao.Deposit(UserID2, ToAmount);
                                Console.Beep();
                                Console.WriteLine("Amount transfered !");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error in Transfer!: " + e.Message);
                            }
                            break;
                        case 4:
                            try
                            {
                                this.accountdao.BalanceCheck(UserID);
                                Console.Beep();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error in BalanceCheck!: " + e.Message);
                            }
                            break;
                        case 5:
                            User[] tran3 = this.accountdao.Transact(UserID);
                            break;
                        case 6:
                            try
                            {
                                int NewPIN = 0;
                                Console.Write("Enter the NewPIN: ");
                                NewPIN = Convert.ToInt32(Console.Read());
                                this.accountdao.PINChange(UserID, NewPIN);
                                Console.Beep();
                                Console.WriteLine("You Changed your PIN Successfully...");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error in PINChange: " + e.Message);
                            }
                            break;

                        case 7:
                            Console.Beep();
                            Console.WriteLine("You Logged out Successfully!");
                            break;
                        default:
                            Console.Beep();
                            Console.WriteLine("Enter a Vaild Input!");
                            break;
                    }
                }
                else
                {
                    Console.Beep();
                    Console.WriteLine("Incorrect UserID or Password!!!");
                }
        }
    }
}