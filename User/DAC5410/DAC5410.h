#ifndef _DAC5410_H_
#define _DAC5410_H_

#include <stdint.h>
#include <stdbool.h>
#include "main.h"
#include "delay.h"

//#define SPI1_NSS_Pin 					GPIO_PIN_14
//#define SPI1_NSS_GPIO_Port 				GPIOB

//#define SPI1_CLK_Pin 					GPIO_PIN_5
//#define SPI1_CLK_GPIO_Port 				GPIOA
//#define SPI1_MISO_Pin 					GPIO_PIN_6
//#define SPI1_MISO_GPIO_Port 			GPIOA
//#define SPI1_MOSI_Pin 					GPIO_PIN_7
//#define SPI1_MOSI_GPIO_Port 			GPIOA
////#define AD5410_FAULT_Pin 				GPIO_PIN_8
////#define AD5410_FAULT_GPIO_Port 			GPIOC

////reset pin
//#define DAC5410_1_Reset_Pin				GPIO_PIN_3
//#define DAC5410_1_Reset_GPIO_Port		GPIOA

//#define DAC5410_2_Reset_Pin				GPIO_PIN_11
//#define DAC5410_2_Reset_GPIO_Port		GPIOB

//#define DAC5410_3_Reset_Pin				GPIO_PIN_13
//#define DAC5410_3_Reset_GPIO_Port		GPIOB

//#define DAC5410_4_Reset_Pin				GPIO_PIN_15
//#define DAC5410_4_Reset_GPIO_Port		GPIOB



#define	Bit13_REXT											  0x0000							//Internal R.
#define	Bit12_OutEN											  0x1000							//Out Enable.
#define	Bit12_OutDisable								  0x0000							//Out Disable.
#define	B11_B8_SRCLK								 0x0F00							//SRCLK=1111(3.3kHz).
#define	B7_B5_SRStep							  		0x00C0							//SR Step=110, 64LSB.
#define	B4_SREN											      0x0010							//=1SR Enable
#define	B3_DCEN_DISABLE							      0x0000							//=0,Dainy Disable
#define	B3_DCEN_ENABLE							      0x0008							//=0,Dainy Disable
#define	B2_B0_R2R1R0										  0x0006							//=110, 0-20mA.
//#define	B2_B0_R2R1R0										  0x0005							//=101, 4-24mA.

#define	DAC_ControlREG_ADDR								0x55
#define	DAC_RSTREG_ADDR									  0x56
#define	DAC_DataREG_ADDR									0x01

#define DAC_NO_DEVICE_1_mA								((float)(65535 * 1.0 / 24.0))
#define DAC_2_mA													((float)(65535 * 2.0 / 24.0))
#define DAC_3_mA													((float)(65535 * 3.0 / 24.0))
#define DAC_4_mA													((float)(65535 * 4.0 / 24.0))

#define DAC5410_LATCH_HIGH()		LL_GPIO_SetOutputPin(AD5410_LATCH_GPIO_Port, AD5410_LATCH_Pin)
#define DAC5410_LATCH_LOW()			LL_GPIO_ResetOutputPin(AD5410_LATCH_GPIO_Port, AD5410_LATCH_Pin)

#define DAC5410_SCLK_HIGH()			LL_GPIO_SetOutputPin(AD5410_SCLK_GPIO_Port, AD5410_SCLK_Pin)
#define DAC5410_SCLK_LOW()			LL_GPIO_ResetOutputPin(AD5410_SCLK_GPIO_Port, AD5410_SCLK_Pin)

#define DAC5410_SIMO_HIGH()			LL_GPIO_SetOutputPin(AD5410_SDIN_GPIO_Port, AD5410_SDIN_Pin)
#define DAC5410_SIMO_LOW()			LL_GPIO_ResetOutputPin(AD5410_SDIN_GPIO_Port, AD5410_SDIN_Pin)

#define DAC5410_CLEAR_HIGH()   		LL_GPIO_SetOutputPin(AD5410_CLEAR_GPIO_Port, AD5410_CLEAR_Pin)
#define DAC5410_CLEAR_LOW()    		LL_GPIO_ResetOutputPin(AD5410_CLEAR_GPIO_Port, AD5410_CLEAR_Pin)


void AD5410_init(void);
void AD5410_IOUT(float DATA1,float DATA2);  //0<DATA<20
	
#endif

