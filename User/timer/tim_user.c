/**
  ******************************************************************************
  * File Name          : TIM.c
  * Description        : This file provides code for the configuration
  *                      of the TIM instances.
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

/* Includes ------------------------------------------------------------------*/
#include "tim_user.h"

/* USER CODE BEGIN 0 */

/* USER CODE END 0 */

/* TIM6 init function */
void MX_TIM6_Modbus_Init(uint16_t PrescalerValue,uint16_t usTim1Timerout50us)
{
  LL_TIM_InitTypeDef TIM_InitStruct = {0};

  /* Peripheral clock enable */
  LL_APB1_GRP1_EnableClock(LL_APB1_GRP1_PERIPH_TIM6);

  /* TIM6 interrupt Init */
  NVIC_SetPriority(TIM6_DAC_IRQn, NVIC_EncodePriority(NVIC_GetPriorityGrouping(),1, 0));
  NVIC_EnableIRQ(TIM6_DAC_IRQn);

  TIM_InitStruct.Prescaler = PrescalerValue;
  TIM_InitStruct.CounterMode = LL_TIM_COUNTERMODE_UP;
  TIM_InitStruct.Autoreload = usTim1Timerout50us;
  LL_TIM_Init(TIM6, &TIM_InitStruct);
  LL_TIM_EnableARRPreload(TIM6);
  LL_TIM_SetTriggerOutput(TIM6, LL_TIM_TRGO_ENABLE);
  LL_TIM_DisableMasterSlaveMode(TIM6);

}

/* USER CODE BEGIN 1 */

/* USER CODE END 1 */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
