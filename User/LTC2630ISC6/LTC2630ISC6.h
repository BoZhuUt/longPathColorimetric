/**
  ******************************************************************************
  * @file    LTC2630ISC6.h
  * @author  bo.zhu
  * @version V1.0
  * @date    2016-12-15
  * @brief   
  ******************************************************************************
  * @attention
  *
  *  
  ******************************************************************************
  */ 
#ifndef __LTC2630ISC6_H__
#define __LTC2630ISC6_H__

#include "main.h"

#define  DAC_CS_HIGH()  			LL_GPIO_SetOutputPin(DAC_Cs_GPIO_Port, DAC_Cs_Pin)
#define  DAC_CS_LOW()				LL_GPIO_ResetOutputPin(DAC_Cs_GPIO_Port, DAC_Cs_Pin)

#define  SPI_SLAVE_IN_BB_HIGH()    	LL_GPIO_SetOutputPin(SPI_SLAVE_IN_GPIO_Port, SPI_SLAVE_IN_Pin)
#define  SPI_SLAVE_IN_BB_LOW()		LL_GPIO_ResetOutputPin(SPI_SLAVE_IN_GPIO_Port, SPI_SLAVE_IN_Pin)

#define SPI_CLOCK_HIGH()   			LL_GPIO_SetOutputPin(SPI_CLOCK_GPIO_Port, SPI_CLOCK_Pin)
#define SPI_CLOCK_LOW()    			LL_GPIO_ResetOutputPin(SPI_CLOCK_GPIO_Port, SPI_CLOCK_Pin)

#define LTC2630ISC6_POWER_DOWN_         0X70
#define LTC2630ISC6_WRITE_TO_AND_UPDATE 0X30

void LTC2630ISC6_init(void);

void write_to_LTC2630ISC6(uint8_t Cmd, uint16_t Dat);

#endif
