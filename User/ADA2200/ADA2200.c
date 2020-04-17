/**
  ******************************************************************************
  * @file    ADA2200.c
  * @author  bo.zhu
  * @version V1.1
  * @date    2016-9-14
  * @brief   
  ******************************************************************************
  * @attention
  *
  *  
  ******************************************************************************
  */ 
#include "ADA2200.h"

u8 DataRead3[3];
void delayLoop2(unsigned int Time)
{
		Time*=3;
		while(Time--)
		{
			__nop();
		}
}
void ada2200_GpioInit()
{
//	GPIO_InitTypeDef g;
//	g.GPIO_Mode=GPIO_Mode_Out_PP;
//	g.GPIO_Speed=GPIO_Speed_50MHz;
//	
//	g.GPIO_Pin=GPIO_Pin_6;// DIN/DIO   PB6
//	GPIO_Init(GPIOB,&g);
//	g.GPIO_Pin=GPIO_Pin_7;//CLK   PB7
//    
//	GPIO_Init(GPIOB,&g);
//	g.GPIO_Pin=GPIO_Pin_8;//CS   PB8
//    
//	GPIO_Init(GPIOB,&g); 
//	g.GPIO_Pin=GPIO_Pin_7;//rst   PA8 
//    
//	GPIO_Init(GPIOA,&g);
//	g.GPIO_Pin=GPIO_Pin_8;//powerPin   PA8 
//	GPIO_Init(GPIOA,&g); 
//    
//	g.GPIO_Mode=GPIO_Mode_IPU;  
//	g.GPIO_Pin=GPIO_Pin_9;    // ADA2200_SYNCO     PB9
//	GPIO_Init(GPIOB,&g);
//	
//	g.GPIO_Mode=GPIO_Mode_IPU;
//	g.GPIO_Pin=GPIO_Pin_0;    // 	ADA2200_RCK    USE TO SHART ADC  20161214 ZB
//	GPIO_Init(GPIOB,&g);
//    
//    ada2200Pwoeron;
//    ada2200Rst_0
//    delay_ms(50);
//    ada2200Rst_1
//ADA2200_Pwoeroff();
//delay_ms(10);
//ADA2200_Pwoeron();
//ADA2200_RST_LOW();
//delay_ms(50);
//ADA2200_RST_HIGH();
 }

void Write3bytesADA(uint16_t Reg ,uint8_t data)
{
//	int i = 0;

//	ADA2200_CS_LOW();
//	for(i = 0; i < 16; i++)
//	{
//		if(Reg & 0x8000)
//		{
//			ADA2200_SPI_SLAVE_IN_HIGH();
//		}
//		else
//		{
//			ADA2200_SPI_SLAVE_IN_LOW();
//		}

//		Reg<<=1;
//		ADA2200_SPI_CLOCK_LOW();
//		delay_us(30);
//		ADA2200_SPI_CLOCK_HIGH();
//		delay_us(30);
//	}
//		for(i=0;i<8;i++)
//	{
//		if((data & 0x80)==0)
//		{        
//			ADA2200_SPI_SLAVE_IN_LOW()	;
//		}    
//		else
//		{
//			ADA2200_SPI_SLAVE_IN_HIGH();
//		}
//		data<<=1;
//		ADA2200_SPI_CLOCK_LOW();
//		delay_us(30);
//		ADA2200_SPI_CLOCK_HIGH();
//		delay_us(30);
//	}
//	ADA2200_CS_HIGH();
}

void ReadFromRegADA(uint16_t Reg,unsigned char nByte) // nByte is the number of bytes which need to be read
{
//	int i,j;
//	unsigned char temp;
//	DIN111;
//	Temp_CS000;
//	temp=0;

//	DataRead3[0]=0;
//	DataRead3[1]=0;
//	DataRead3[2]=0;
////	DataRead[3]=0;
//		for(i=0; i<16; i++)
//	{
//		if(Reg & 0x8000)
//		{
//			DIN111;
//		}
//		else
//		{
//			DIN000;
//		}

//		Reg<<=1;
//		SCLOCK000;
//		delayLoop2(10);
//		SCLOCK111;
//		delayLoop2(10);
//	}
//	for(i=0; i<nByte; i++)
//	{
//		for(j=0; j<8; j++)
//		{
//			SCLOCK000;
//			if(GPIO_ReadInputDataBit(GPIOB,GPIO_Pin_1)==0)
//			{
//				temp=temp<<1;
//			}else
//			{
//				temp=temp<<1;
//				temp=temp+0x01;
//			}
//			delayLoop2(10);
//			SCLOCK111;
//			delayLoop2(10);
//		}
//		DataRead3[i]=temp;
//		temp=0;
//	}
//	Temp_CS111;
}
void ADA2200_Init()
{
	ada2200_GpioInit();
	delay_ms(1);
	
// 	Write2bytesADA(0X0000); //写串行接口
// 	WritebyteADA(0X24);   //MSB优先 SDIO双向  SDO禁用  地址递增
	
 	Write3bytesADA(0X0010,0X01);
	//delay_ms(40);
//重构滤波器 BP1
    
    
    Write3bytesADA(0X0011,0XC0);
    Write3bytesADA(0X0012,0X0F);
    Write3bytesADA(0X0013,0X36);
    Write3bytesADA(0X0014,0XD1);
    Write3bytesADA(0X0015,0XC0);
    Write3bytesADA(0X0016,0X0F);
    Write3bytesADA(0X0017,0X07);
    Write3bytesADA(0X0018,0X80);
    Write3bytesADA(0X0019,0X07);
    Write3bytesADA(0X001A,0X80);
    
    Write3bytesADA(0X001B,0X00);
    Write3bytesADA(0X001C,0X20);
    Write3bytesADA(0X001D,0XC0); 
    Write3bytesADA(0X001E,0X4F);
    Write3bytesADA(0X001F,0XAA);
    Write3bytesADA(0X0020,0XAA);
    Write3bytesADA(0X0021,0XC0);
    Write3bytesADA(0X0022,0X0F);
    Write3bytesADA(0X0023,0XC0);
    Write3bytesADA(0X0024,0X4F);
    Write3bytesADA(0X0025,0X23);
    Write3bytesADA(0X0026,0X02);
    Write3bytesADA(0X0027,0X02);
    
		Write3bytesADA(0X0010,0X03);
		Write3bytesADA(0X0010,0X01);
	
// 	Write2bytesADA(0X0028); // 写模拟引脚配置  
// 	WritebyteADA(0X01); // 外部时钟 差分输入
	Write3bytesADA(0X0028,0X03);
	delay_ms(40);
	
// 	Write2bytesADA(0X0029);   //同步控制
// 	WritebyteADA(0X0D);     //SYNCO close
	Write3bytesADA(0X0029,0X31); //0x38
	delay_ms(40);
	
// 	Write2bytesADA(0X002A);  //解调控制
// 	WritebyteADA(0X18);      //移相关闭 RCLK使能 VOCM=0.5VDD
    Write3bytesADA(0X002A,0X0A); //0x18
	delay_ms(40);
	
// 	Write2bytesADA(0X002B);  //时钟配置
// 	WritebyteADA(0X12);      //Fsi=1/1 Fclk   Fm=1/8 Fso  
	Write3bytesADA(0X002B,0X11);  //Fsi=19.23M/64, Fso=Fsi/8, RCLK=Fso/4=	2.929khz
	delay_ms(40);
	
// 	Write2bytesADA(0X002C); //数字引脚配置
// 	WritebyteADA(0X01);     //RCLK输出焊盘驱动器使能
	Write3bytesADA(0X002C,0X01);
	delay_ms(40);
// 	ReadFromRegAD(0X802B,1);
}
