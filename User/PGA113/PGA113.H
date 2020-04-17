#ifndef __PGA113_H__
#define __PGA113_H__

//#include "stm32f10x.h"
#include "main.h"
#include "delay.h"
//#define  PGA113_CS_PIN          GPIO_Pin_10
//#define  PGA113_CS_PORT         GPIOB
//#define  PGA113_DIO_PIN         GPIO_Pin_6
//#define  PGA113_DIO_PORT        GPIOB
//#define  PGA113_CLK_PIN         GPIO_Pin_7
//#define  PGA113_CLK_PORT        GPIOB

#define  PGA113_CS_H      LL_GPIO_SetOutputPin(SPI_MOSI_PGA_GPIO_Port, SPI_MOSI_PGA_Pin)
#define  PGA113_CS_L      LL_GPIO_ResetOutputPin(SPI_MOSI_PGA_GPIO_Port, SPI_MOSI_PGA_Pin)

#define  PGA113_DIO_H     LL_GPIO_SetOutputPin(PGA113_Cs_GPIO_Port, PGA113_Cs_Pin)
#define  PGA113_DIO_L     LL_GPIO_ResetOutputPin(PGA113_Cs_GPIO_Port, PGA113_Cs_Pin)

#define  PGA113_CLK_H     LL_GPIO_SetOutputPin(SPI_CLOCK_PGA_GPIO_Port, SPI_CLOCK_PGA_Pin)
#define  PGA113_CLK_L     LL_GPIO_ResetOutputPin(SPI_CLOCK_PGA_GPIO_Port, SPI_CLOCK_PGA_Pin)
#define ch0 0x00
#define ch1 0x01
//pga112
#define pga112Gain1     0x00
#define pga112Gain2     0x10
#define pga112Gain4     0x20
#define pga112Gain8     0x30
#define pga112Gain16    0x40
#define pga112Gain32    0x50
#define pga112Gain64    0x60
#define pga112Gain128   0x70

//pga113
#define pga113Gain1     0
#define pga113Gain2     1
#define pga113Gain5     2
#define pga113Gain10    3
#define pga113Gain20    4
#define pga113Gain50    5
#define pga113Gain100   6
#define pga113Gain200   7

void PGA113_GPIOInit(void);
void configPGA113(uint8_t ch,uint8_t gain);

#endif
