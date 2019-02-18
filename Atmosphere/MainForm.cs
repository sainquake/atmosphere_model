/*
 * Created by SharpDevelop.
 * User: SAINQUAKE
 * Date: 12.12.2013
 * Time: 22:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ODE01;

namespace Atmosphere
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	/// 
	
		
		
	public partial class MainForm : Form
	{
		CDataIO io=new CDataIO();
		uint N = 1000;
		
		public double H = 1.000;
		public double H0 = 0;
		public double D = 3.000;
		public double lambda1 = 3;
		public double lambda2 = 5;
		//
		public double sigma0;
		public double h0;
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			lambda = new double[88];
			int i =0;
			for(double l=0.4;l<5.5;l+=0.1){
				lambda[i] = l;
				i++;
			}
			for(double l=7;l<14;l+=0.2){
				lambda[i] = l;
				i++;
			}
			
			Trarr = new double[N];
			Trnarr = new double[N];
			Tryarr = new double[N];
			Trsarr = new double[N];
			larr = new double[N];
						
			TrSumm();
			label15.Text = Tr(Convert.ToDouble(textBox9.Text)).ToString();
			label20.Text = Trn(Convert.ToDouble(textBox10.Text)).ToString();
			label24.Text = Try(Convert.ToDouble(textBox11.Text)).ToString();
			label45.Text = (Tr(Convert.ToDouble(textBox9.Text))*Trn(Convert.ToDouble(textBox10.Text))*Try(Convert.ToDouble(textBox12.Text))).ToString();
		}
		
		//aerosol
		double Tr1=1;
		double Tr2=1;
		double[] Trarr;
		double[] larr;
		void TrAero(){
			sigma0 = 3.9/Sm;
			label10.Text = sigma0.ToString();
			//h0
			h0 = 0.78 + 0.038*Sm;
			
			for(int i=0;i<N;i++){
				larr[i] = lambda1+i*(lambda2-lambda1)/1000;
				Trarr[i] = Tr(larr[i]);
			}
			io.WriteArrayToFile("Tr.dat",(uint)N,Trarr);
			io.WriteArrayToFile("lambda.dat",(uint)N,larr);
		}
		void Button4Click(object sender, EventArgs e)
		{
			TrAero();
			io.ShowDataImager("DataImager2.2.exe","lambda Tr");
		}
		
		double Lp(double lam){
			return alpha(lam)*sigma0*D*(Kpi(H0)-Kpi(H))/(H0-H);
		}
		double Sigma0(double h){
			return sigma0*Math.Exp(-h/h0);
		}
		double Kpi(double Hi){
			return h0*(1-Math.Exp(-Hi/h0));
		}
		double alpha(double lam){
			return Math.Pow((0.55/lam),0.585*Math.Pow(Sm,1/3));
		}
		double Tr(double lam){
			return Math.Exp(-Lp(lam));
		}
		//H20
		double Trn1 =1;
		double Trn2 = 1;
		double[] Trnarr;
		void TrH2O(){
			for(int i=0;i<N;i++){
				larr[i] = lambda1+i*(lambda2-lambda1)/1000;
				Trnarr[i] = Trn(larr[i]);
			}
			io.WriteArrayToFile("Trn.dat",(uint)N,Trnarr);
			io.WriteArrayToFile("lambda.dat",(uint)N,larr);
		}
		void Button5Click(object sender, EventArgs e)
		{
			TrH2O();
			io.ShowDataImager("DataImager2.2.exe","lambda Trn");
		}
		void Button2Click(object sender, System.EventArgs e)
		{
			Trn1 = Trn(lambda1);
			Trn2 = Trn(lambda2);
			label22.Text = Trn(lambda1).ToString();
			label23.Text = Trn(lambda2).ToString();
			CalcSumm();
		}
		double Trn(double lam){
			int i=0;
			for(i=0;i<lambda.Length-1;i++)
				if(lam>=lambda[i] && lam<=lambda[i+1])
					break;
			double tn0_ = linearInt(lam,tn0[i],lambda[i],tn0[i+1],lambda[i+1]);//(tn0[i]+tn0[ (i+1) ])/2;
			double W = 0.02168*fotn*(-0.000311*tv*tv+0.0738*tv+6.41)/(273+tv);
			double Qn = ((Kni(H0)-Kni(H))/(H0-H))*D*W;
			//if(lam>=3 && lam<=5)
				//Qn = Math.Pow(Qn,0.53);
			return Math.Pow(tn0_,Qn);
		}
		double Kni(double Hi){
			return 2.2*(1-Math.Exp(-Hi/2.2));
		}
		
		double linearInt(double tlam,double t1,double l1,double t2,double l2){
			double K = (t1-t2)/(l1-l2);
			double B = t1-K*l1;
			return tlam*K+B;
		}
		//CO2
		double Try1=1;
		double Try2=1;
		double[] Tryarr;
		void TrCO2(){
			for(int i=0;i<N;i++){
				larr[i] = lambda1+i*(lambda2-lambda1)/1000;
				Tryarr[i] = Try(larr[i]);
			}
			io.WriteArrayToFile("Try.dat",(uint)N,Tryarr);
			io.WriteArrayToFile("lambda.dat",(uint)N,larr);
		}
		void Button6Click(object sender, EventArgs e)
		{
			TrCO2();
			io.ShowDataImager("DataImager2.2.exe","lambda Try");
		}
		void Button3Click(object sender, EventArgs e)
		{
			Try1 = Try(lambda1);
			Try2 = Try(lambda2);
			label25.Text = Try(lambda1).ToString();
			label24.Text = Try(lambda2).ToString();
			CalcSumm();
		}
		double Try(double lam){
			int i=0;
			for(i=0;i<lambda.Length-1;i++)
				if(lam>=lambda[i] && lam<=lambda[i+1])
					break;
			double ty0_ = linearInt(lam,ty0[i],lambda[i],ty0[i+1],lambda[i+1]);
			double Qn = ((Kyi(H0)-Kyi(H))/(H0-H))*D;
			//if(lam>=3 && lam<=5)
				//Qn = Math.Pow(Qn,0.64);
			return Math.Pow(ty0_,Qn);
		}
		double Kyi(double Hi){
			return 7*(1-Math.Exp(-Hi/7));
		}
		//Summ
		void CalcSumm(){
			label26.Text = (Try1*Trn1*Tr1).ToString();
			label27.Text = (Try2*Trn2*Tr2).ToString();
		}
		double[] Trsarr;
		void TrSumm(){
			TrAero();
			TrH2O();
			TrCO2();
			for(int i=0;i<N;i++){
				larr[i] = lambda1+i*(lambda2-lambda1)/1000;
				Trsarr[i] = Tr(larr[i])*Trn(larr[i])*Try(larr[i]);
			}
			io.WriteArrayToFile("Trs.dat",(uint)N,Trsarr);
			io.WriteArrayToFile("lambda.dat",(uint)N,larr);
		}
		void Button7Click(object sender, EventArgs e)
		{
			TrSumm();
			io.ShowDataImager("DataImager2.2.exe","lambda Trs");
		}
		double TrS(double lam){
			return Tr(lam)*Trn(lam)*Try(lam);
		}
		//
		public double[] lambda = new double[87];
		public double[] tn0 = new double[]{
			0.937,0.956,0.968,0.972,0.965,0.890,0.968,0.905,0.937,0.268,
			0.782,0.988,0.994,0.994,0.406,0.874,0.953,0.988,0.994,0.988,
			0.937,0.782,0.11,0.004,0.017,0.025,0.552,0.692,0.765,0.843,//30
			0.914,0.962,0.982,0.998,0.994,0.994,0.990,0.988,0.982,0.972,
			0.937,0.905,0.874,0.843,0.812,0.782,0.736,0.649,0.539,0.406,
			0.268,0.11,0.06,0.292,0.666,0.666,0.878,0.951,0.964,0.975,//60
			0.982,0.983,0.984,0.985,0.986,0.987,0.987,0.988,0.988,0.988,
			0.988,0.988,0.988,0.986,0.986,0.987,0.982,0.987,0.987,0.987,
			0.986,0.985,0.984,0.982,0.981,0.979,0.978,0.977
			};
		public double[] ty0 = new double[]{
			1,1,1,1,1,1,1,1,1,1,
			0.988,0.998,0.998,0.999,1,0.999,0.931,0.994,1,1,
			1,1,1,0.419,0.578,0.990,1,1,1,1,
			1,1,1,1,1,1,0.994,0.944,0.182,0,
			0.026,0.863,0.985,0.985,0.922,0.920,0.915,0.928,0.955,0.989,
			1,1,1,1,1,1,1,1,1,1,
			1,1,1,0.999,0.965,0.980,0.984,0.999,0.988,0.999,
			0.999,0.999,0.999,0.998,0.997,0.995,0.997,0.999,0.999,0.970,
			0.903,0.949,0.955,0.895,0.715,0.627,0.464,0.286
		};
		
		
		//Meteo selected
		
		public double fotn = 70;
		public double tv = 23;
		public double Sm = 3.000;
		void ComboBox1SelectedIndexChanged(object sender, System.EventArgs e)
		{
			label29.Text = comboBox1.SelectedIndex.ToString();
			switch(comboBox1.SelectedIndex){
			case 0:
				label38.Text = "(0..100)";
				label30.Text = "(-50..+50)";
				label39.Text = "(0..100)";
				Sm = 10;
			break;
			case 1:
				label38.Text = "(30..100)";
				label30.Text = "(-35..+25)";
				label39.Text = "(0..50)";
				tv = 10;
				fotn = 65;
				Sm = 25;
			break;
			case 2:
				label38.Text = "(60..100)";
				label30.Text = "(-12..+25)";
				label39.Text = "(1..15)";
				tv = 10;
				fotn = 80;
				Sm = 8;
			break;
			case 3:
				label38.Text = "(90..100)";
				label30.Text = "(0..+25)";
				label39.Text = "(3..10)";
				tv = 12;
				fotn = 95;
				Sm = 6;
			break;
			case 4://rain
				label38.Text = "(75..100)";
				label30.Text = "(0..+25)";
				label39.Text = "(3..15)";
				tv = 12;
				fotn = 87;
				Sm = 9;
			break;
			case 5://snow
				label38.Text = "(50..100)";
				label30.Text = "(-50..0)";
				label39.Text = "(5..15)";
				tv = -25;
				fotn = 75;
				Sm = 10;
			break;
			}
			
				textBox8.Text = tv.ToString();
				textBox7.Text = fotn.ToString();
				textBox4.Text = Sm.ToString();
				TrSumm();
		}
				
		void ComboBox2SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch(comboBox2.SelectedIndex){
			case 0:
				lambda1 =0.4;
				lambda2 =14;
			break;
			case 1:
				lambda1 =0.4;
				lambda2 =0.7;
			break;
			case 2:
				lambda1 =1.4;
				lambda2 =2.4;
			break;
			case 3:
				lambda1 =3;
				lambda2 =5;
			break;
			case 4:
				lambda1 =8;
				lambda2 =14;
			break;
			}
			textBox5.Text = lambda1.ToString();
			textBox6.Text = lambda2.ToString();
			TrSumm();
		}
		
		void TextBox5TextChanged(object sender, EventArgs e)
		{
			lambda1 = Convert.ToDouble(textBox5.Text);
			TrSumm();
		}
		
		void TextBox6TextChanged(object sender, EventArgs e)
		{
			lambda2 = Convert.ToDouble(textBox6.Text);
			TrSumm();
		}
		
		void TextBox1TextChanged(object sender, EventArgs e)
		{
			D = Convert.ToDouble(textBox1.Text);
			TrSumm();
		}
		
		void TextBox2TextChanged(object sender, EventArgs e)
		{
			H0 = Convert.ToDouble(textBox2.Text);
			TrSumm();
		}
		
		void TextBox3TextChanged(object sender, EventArgs e)
		{
			H = Convert.ToDouble(textBox3.Text);
			TrSumm();
		}
		
		void TextBox4TextChanged(object sender, EventArgs e)
		{
			Sm = Convert.ToDouble(textBox4.Text);
			sigma0 = 3.9/Sm;
			label10.Text = sigma0.ToString();
			//h0
			h0 = 0.78 + 0.038*Sm;
			TrSumm();
		}
		
		void TextBox8TextChanged(object sender, EventArgs e)
		{
			tv = Convert.ToDouble(textBox8.Text);
			TrSumm();
		}
		
		void TextBox7TextChanged(object sender, EventArgs e)
		{
			fotn = Convert.ToDouble(textBox7.Text);
			TrSumm();
		}
		
		
		
		void TextBox9TextChanged(object sender, EventArgs e)
		{
			label15.Text = Tr(Convert.ToDouble(textBox9.Text)).ToString();
		}
		
		void TextBox10TextChanged(object sender, EventArgs e)
		{
			label20.Text = Trn(Convert.ToDouble(textBox10.Text)).ToString();
		}
		
		void TextBox11TextChanged(object sender, EventArgs e)
		{
			label24.Text = Try(Convert.ToDouble(textBox11.Text)).ToString();
		}
		
		void TextBox12TextChanged(object sender, EventArgs e)
		{
			label45.Text = (Tr(Convert.ToDouble(textBox9.Text))*Trn(Convert.ToDouble(textBox10.Text))*Try(Convert.ToDouble(textBox12.Text))).ToString();
		}
		//MTF
		double lambda0 = 4;
		double a0=10;
		
		double[] v = new double[1000];
		double[] T_ = new double[1000];
		void Button1Click(object sender, EventArgs e)
		{
			for(int i=0;i<N;i++){
				v[i] = Convert.ToDouble(i)/100;
				T_[i] = T(v[i]);
			}
			io.WriteArrayToFile("T.dat",(uint)N,T_);
			io.WriteArrayToFile("v.dat",(uint)N,v);
			io.ShowDataImager("DataImager2.2.exe","v T");
		}
		void TextBox13TextChanged(object sender, EventArgs e)
		{
			lambda0 = Convert.ToDouble(textBox13.Text);
		}
		
		void ComboBox3SelectedIndexChanged(object sender, EventArgs e)
		{
			switch(comboBox3.SelectedIndex){
			case 0:
				a0 = 10;	
			break;
			case 1:
				a0 = 0.2;	
			break;
			case 2:
				a0 = 3;	
			break;
			case 3:
				a0 = 650;	
			break;
			}
		}
		double T(double v){
			return (1-TrS(lambda0))*TrS(lambda0)*Math.Exp(-(v*lambda0/a0)*(v*lambda0/a0))+TrS(lambda0);
		}
	}
}
