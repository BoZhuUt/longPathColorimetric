/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.h
  * @brief          : Header for main.c file.
  *                   This file contains the common defines of the application.
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; Copyright (c) 2020 STMicroelectronics.
  * All rights reserved.</center></h2>
  *
  * This software component is licensed by ST under BSD 3-Clause license,
  * the "License"; You may not use this file except in compliance with the
  * License. You may obtain a copy of the License at:
  *                        opensource.org/licenses/BSD-3-Clause
  *
  ******************************************************************************
  */
/* USER CODE END Header */

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __MAIN_H
#define __MAIN_H

#ifdef __cplusplus
extern "C" {
#endif

/* Includes ------------------------------------------------------------------*/
#include "stm32l4xx_ll_adc.h"
#include "stm32l4xx_ll_crs.h"
#include "stm32l4xx_ll_rcc.h"
#include "stm32l4xx_ll_bus.h"
#include "stm32l4xx_ll_system.h"
#include "stm32l4xx_ll_exti.h"
#include "stm32l4xx_ll_cortex.h"
#include "stm32l4xx_ll_utils.h"
#include "stm32l4xx_ll_pwr.h"
#include "stm32l4xx_ll_dma.h"
#include "stm32l4xx_ll_tim.h"
#include "stm32l4xx.h"
#include "stm32l4xx_ll_gpio.h"

#if defined(USE_FULL_ASSERT)
#include "stm32_assert.h"
#endif /* USE_FULL_ASSERT */

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */
#include "stm32l4xx_ll_tim.h"
#include "stm32l4xx_ll_usart.h"

#include "tim_user.h"
#include "usart_user.h"
#include "regmap.h"
#include "LTC2630ISC6.h"
#include "delay.h"
#include "ADA2200.h"
#include "PGA113.h"
#include "DAC5410.h"
#include "app.h"
#include "flash.h"
/* USER CODE END Includes */

/* Exported types ------------------------------------------------------------*/
/* USER CODE BEGIN ET */

/* USER CODE END ET */

/* Exported constants --------------------------------------------------------*/
/* USER CODE BEGIN EC */
extern LL_RCC_ClocksTypeDef RCC_Clocks;
extern MEASURE_VALUES_T measure_values;
extern MEASURE_SETTINGS_T measure_settings;
extern SYS_STATUS_T system_status;
extern PH_ORP_PARAM_T ph_orp_param;
extern COMM_SETTINGS_T  comm_settings;
extern PH_ORP_PARAM_T  pH_ORP_Param;
extern FILTER_SETTINGS_T filter_settings;
extern CALIB_SETTINGS_T calib_settings;
extern uint8_t flag,flag1;
/* USER CODE END EC */

/* Exported macro ------------------------------------------------------------*/
/* USER CODE BEGIN EM */

/* USER CODE END EM */

/* Exported functions prototypes ---------------------------------------------*/
void Error_Handler(void);

/* USER CODE BEGIN EFP */

/* USER CODE END EFP */

/* Private defines -----------------------------------------------------------*/
#define SPI_MOSI_PGA_Pin LL_GPIO_PIN_1
#define SPI_MOSI_PGA_GPIO_Port GPIOA
#define SPI_CLOCK_PGA_Pin LL_GPIO_PIN_2
#define SPI_CLOCK_PGA_GPIO_Port GPIOA
#define PGA113_Cs_Pin LL_GPIO_PIN_3
#define PGA113_Cs_GPIO_Port GPIOA
#define LED_SWITCH_2_Pin LL_GPIO_PIN_6
#define LED_SWITCH_2_GPIO_Port GPIOA
#define LED_SWITCH_1_Pin LL_GPIO_PIN_7
#define LED_SWITCH_1_GPIO_Port GPIOA
#define DAC_Cs_Pin LL_GPIO_PIN_0
#define DAC_Cs_GPIO_Port GPIOB
#define SPI_CLOCK_Pin LL_GPIO_PIN_1
#define SPI_CLOCK_GPIO_Port GPIOB
#define SPI_SLAVE_IN_Pin LL_GPIO_PIN_2
#define SPI_SLAVE_IN_GPIO_Port GPIOB
#define RS485_DERE_Pin LL_GPIO_PIN_12
#define RS485_DERE_GPIO_Port GPIOA
#ifndef NVIC_PRIORITYGROUP_0
#define NVIC_PRIORITYGROUP_0         ((uint32_t)0x00000007) /*!< 0 bit  for pre-emption priority,
                                                                 4 bits for subpriority */
#define NVIC_PRIORITYGROUP_1         ((uint32_t)0x00000006) /*!< 1 bit  for pre-emption priority,
                                                                 3 bits for subpriority */
#define NVIC_PRIORITYGROUP_2         ((uint32_t)0x00000005) /*!< 2 bits for pre-emption priority,
                                                                 2 bits for subpriority */
#define NVIC_PRIORITYGROUP_3         ((uint32_t)0x00000004) /*!< 3 bits for pre-emption priority,
                                                                 1 bit  for subpriority */
#define NVIC_PRIORITYGROUP_4         ((uint32_t)0x00000003) /*!< 4 bits for pre-emption priority,
                                                                 0 bit  for subpriority */
#endif
/* USER CODE BEGIN Private defines */
#define RS485_SEND_EN()			LL_GPIO_SetOutputPin(RS485_DERE_GPIO_Port, RS485_DERE_Pin)
#define RS485_RECEIVE_EN()		LL_GPIO_ResetOutputPin(RS485_DERE_GPIO_Port, RS485_DERE_Pin)

#define LED_SWITCH_A0_HIGH()	LL_GPIO_SetOutputPin(LED_SWITCH_1_GPIO_Port, LED_SWITCH_1_Pin)
#define LED_SWITCH_A0_LOW()		LL_GPIO_ResetOutputPin(LED_SWITCH_1_GPIO_Port, LED_SWITCH_1_Pin)

#define LED_SWITCH_A1_HIGH()	LL_GPIO_SetOutputPin(LED_SWITCH_2_GPIO_Port, LED_SWITCH_2_Pin)
#define LED_SWITCH_A1_LOW()		LL_GPIO_ResetOutputPin(LED_SWITCH_2_GPIO_Port, LED_SWITCH_2_Pin)


#define PD_SWITCH_A0_HIGH()		LL_GPIO_SetOutputPin(PD_SELECT1_GPIO_Port, PD_SELECT1_Pin)
#define PD_SWITCH_A0_LOW()		LL_GPIO_ResetOutputPin(PD_SELECT1_GPIO_Port, PD_SELECT1_Pin)

#define PD_SWITCH_A1_HIGH()		LL_GPIO_SetOutputPin(PD_SELECT2_GPIO_Port, PD_SELECT2_Pin)
#define PD_SWITCH_A1_LOW()		LL_GPIO_ResetOutputPin(PD_SELECT2_GPIO_Port, PD_SELECT2_Pin)

#define SELECT_LED3_EMISSION()	LL_GPIO_ResetOutputPin(SELECT_LED3_GPIO_Port, SELECT_LED3_Pin)
#define SELECT_LED3_PD()		LL_GPIO_SetOutputPin(SELECT_LED3_GPIO_Port, SELECT_LED3_Pin)
/* USER CODE END Private defines */

#ifdef __cplusplus
}
#endif

#endif /* __MAIN_H */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
