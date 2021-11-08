#include "PGA113.h"

void PGA113_GPIOInit(void)
{
//	GPIO_InitTypeDef g;
//	g.GPIO_Mode=GPIO_Mode_Out_PP;
//	g.GPIO_Speed=GPIO_Speed_50MHz;
//	
//	g.GPIO_Pin=PGA113_CS_PIN | PGA113_DIO_PIN;   
//	GPIO_Init(PGA113_DIO_PORT,&g);
//    
//	g.GPIO_Pin=PGA113_CLK_PIN;   
//	GPIO_Init(PGA113_CLK_PORT,&g); 
    PGA113_CLK_H;    
}

void WriteByte(uint8_t command)
{
    unsigned char i;
    for(i = 0; i < 8;i++)
    {
        PGA113_CLK_L;
        delay_us(10);

        if(command&0x80)
          PGA113_DIO_H;
        else
          PGA113_DIO_L;
          command <<= 1;
          delay_us(10);
          
          PGA113_CLK_H;
          delay_us(10);
        //   PGA113_CLK_L;	
        //   delay_us(10);        
    }
}

void configPGA113(uint8_t ch,uint8_t gain)
{
    unsigned char i;
    i=gain<<4;
    PGA113_CS_L;
    WriteByte(0x2A);
    WriteByte(i);
    PGA113_CS_H;
}
void configPGA112(uint8_t ch,uint8_t gain)
{
    unsigned char i;
    i = ch+gain;
    PGA113_CS_L;
    WriteByte(0x2A);
    WriteByte(i);
    PGA113_CS_H;
}




