/**
  ******************************************************************************
  * @file    ADA2200.h
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

#ifndef __ADA2200_H__
#define __ADA2200_H__

//#include "stm32f10x.h"
//#include "sys.h"
//#include "delay.h"
#include "main.h"


#define  ADA2200_CS_HIGH()  			LL_GPIO_SetOutputPin(ADA2200_CS_GPIO_Port, ADA2200_CS_Pin)
#define  ADA2200_CS_LOW()				LL_GPIO_ResetOutputPin(ADA2200_CS_GPIO_Port, ADA2200_CS_Pin)

#define ADA2200_SPI_CLOCK_HIGH()   			LL_GPIO_SetOutputPin(SPI_CLOCK_GPIO_Port, SPI_CLOCK_Pin)
#define ADA2200_SPI_CLOCK_LOW()    			LL_GPIO_ResetOutputPin(SPI_CLOCK_GPIO_Port, SPI_CLOCK_Pin)


#define  ADA2200_SPI_SLAVE_IN_HIGH()    	LL_GPIO_SetOutputPin(SPI_SLAVE_IN_GPIO_Port, SPI_SLAVE_IN_Pin)
#define  ADA2200_SPI_SLAVE_IN_LOW()		LL_GPIO_ResetOutputPin(SPI_SLAVE_IN_GPIO_Port, SPI_SLAVE_IN_Pin)

//#define ada2200Pwoeron     	 GPIO_SetBits(GPIOA,GPIO_Pin_8);
//#define ada2200Pwoeroff      GPIO_ResetBits(GPIOA,GPIO_Pin_8);

#define ADA2200_Pwoeron()     	 LL_GPIO_SetOutputPin(S2B_GPIO_Port, S2B_Pin)
#define ADA2200_Pwoeroff()      LL_GPIO_ResetOutputPin(S2B_GPIO_Port, S2B_Pin)

//#define ada2200Rst_1     	 GPIO_SetBits(GPIOA,GPIO_Pin_7);
//#define ada2200Rst_0     	 GPIO_ResetBits(GPIOA,GPIO_Pin_7);

#define ADA2200_RST_HIGH()     	 LL_GPIO_SetOutputPin(ADA2200_RST_GPIO_Port, ADA2200_RST_Pin)
#define ADA2200_RST_LOW()     	 LL_GPIO_ResetOutputPin(ADA2200_RST_GPIO_Port, ADA2200_RST_Pin)

 void ADA2200_Init(void);


#endif
