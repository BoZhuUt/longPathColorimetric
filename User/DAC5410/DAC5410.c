#include <stdbool.h>
#include <stdint.h>

#include "DAC5410.h"


 /******************************/
 /*   往AD5410中写入一个字节   */
 /******************************/
 void DA_SendByte(unsigned char byte)  
{		
		
    unsigned char i;
    for ( i = 0; i < 8; i++)
    {
//        DAC5410_SCLK_LOW(); 
//        if ( byte & 0x80 )
//				{
//					DAC5410_SIMO_HIGH();
//					delay_us(1);
//				}
//        else
//				{
//					DAC5410_SIMO_LOW();
//					delay_us(1);
//				}
//        DAC5410_SCLK_HIGH(); 
//        byte <<= 1;     
//        DAC5410_SCLK_LOW();
    }
}
/***********************************/
/*   往AD5410中写入完整的命令      */
/*   总长3个字节 addr寄存器地址    */
/*               data操作指令      */
 void DA_control(unsigned char addr,unsigned int data)    
{
 uint8_t Hdata,Ldata;
 
//	Hdata=data>>8;
//	Ldata=data&255;
//	DAC5410_LATCH_LOW();
//	DA_SendByte(addr);
//	DA_SendByte(Hdata);
//	DA_SendByte(Ldata);
//	DAC5410_LATCH_HIGH();
//	delay_us(2);
}
 void DA_control2(unsigned char addr,unsigned int data,unsigned int data2)    
{
// uint8_t Hdata,Ldata;
//	 uint8_t Hdata2,Ldata2;
// 
//	Hdata = data>>8;
//	Ldata = data&255;
//	
//	Hdata2 = data2>>8;
//	Ldata2 = data2&255;
//	
//	
//	DAC5410_LATCH_LOW();
//	DA_SendByte(addr);
//	DA_SendByte(Hdata);
//	DA_SendByte(Ldata);
//	DA_SendByte(addr);
//	DA_SendByte(Hdata2);
//	DA_SendByte(Ldata2);
//	DAC5410_LATCH_HIGH();
//	delay_us(2);
}
 
// void LATCH()
// {
//	 DAC5410_LATCH_LOW();
//	 delay_us(5);
//	 DAC5410_LATCH_HIGH();
//	 delay_us(5);
//	 DAC5410_LATCH_LOW();

//}
/**********************/
/*    初始化AD5410    */
void AD5410_init()
{
//	DA_control(0x56,0x0001);   //复位  0x56复位寄存器地址   0x0001复位指令
//	DAC5410_CLEAR_LOW();

//	DA_control(0x55,0x101e);   //0-20ma  257730更新频率   1/16步进大小

//	DA_control2(0x56,0x0001,0x0000);
//	DA_control2(0x55,0x1016,0x101e);   //0-20ma  257730更新频率   1/16步进大小
}
/********************/
/* IOUT  0<=DATA<=20*/
void AD5410_IOUT(float DATA1,float DATA2)
{
	uint16_t I_OUT1,I_OUT2;
	if(DATA1>19.99f) //float不可以用>=
	{
		I_OUT1=4095;
	}
	else if(DATA1<4.0f)
	{
		DATA1=4;
		I_OUT1=DATA1*4096/20;
	}
	else
	{
		I_OUT1=DATA1*4096/20;
	}	
	if(DATA2>19.99f) //float不可以用>=
	{
		I_OUT2=4095;
	}
	else if(DATA2<4.0f)
	{
		DATA2=4;
		I_OUT2=DATA2*4096/20;
	}
	else
	{
		I_OUT2=DATA2*4096/20;
	}
	
	I_OUT1=((int)I_OUT1)<<4;
	I_OUT2=((int)I_OUT2)<<4;
//	DA_control(0x01,I_OUT);///往AD5410中写入完整的命令
	DA_control2(0x01,I_OUT1,I_OUT2);
//	LATCH();

}






