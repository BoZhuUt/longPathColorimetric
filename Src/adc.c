/**
  ******************************************************************************
  * File Name          : ADC.c
  * Description        : This file provides code for the configuration
  *                      of the ADC instances.
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
#include "adc.h"

/* USER CODE BEGIN 0 */

/* USER CODE END 0 */

/* ADC1 init function */
void MX_ADC1_Init(void)
{
  LL_ADC_InitTypeDef ADC_InitStruct = {0};
  LL_ADC_REG_InitTypeDef ADC_REG_InitStruct = {0};
  LL_ADC_CommonInitTypeDef ADC_CommonInitStruct = {0};

  LL_GPIO_InitTypeDef GPIO_InitStruct = {0};

  /* Peripheral clock enable */
  LL_AHB2_GRP1_EnableClock(LL_AHB2_GRP1_PERIPH_ADC);
  
  LL_AHB2_GRP1_EnableClock(LL_AHB2_GRP1_PERIPH_GPIOA);
  /**ADC1 GPIO Configuration  
  PA4   ------> ADC1_IN9 
  */
  GPIO_InitStruct.Pin = LL_GPIO_PIN_4;
  GPIO_InitStruct.Mode = LL_GPIO_MODE_ANALOG;
  GPIO_InitStruct.Pull = LL_GPIO_PULL_NO;
  LL_GPIO_Init(GPIOA, &GPIO_InitStruct);

  /** Common config 
  */
  ADC_InitStruct.Resolution = LL_ADC_RESOLUTION_12B;
  ADC_InitStruct.DataAlignment = LL_ADC_DATA_ALIGN_RIGHT;
  ADC_InitStruct.LowPowerMode = LL_ADC_LP_MODE_NONE;
  LL_ADC_Init(ADC1, &ADC_InitStruct);
  ADC_REG_InitStruct.TriggerSource = LL_ADC_REG_TRIG_SOFTWARE;
  ADC_REG_InitStruct.SequencerLength = LL_ADC_REG_SEQ_SCAN_DISABLE;
  ADC_REG_InitStruct.SequencerDiscont = LL_ADC_REG_SEQ_DISCONT_DISABLE;
  ADC_REG_InitStruct.ContinuousMode = LL_ADC_REG_CONV_SINGLE;
  ADC_REG_InitStruct.DMATransfer = LL_ADC_REG_DMA_TRANSFER_NONE;
  ADC_REG_InitStruct.Overrun = LL_ADC_REG_OVR_DATA_PRESERVED;
  LL_ADC_REG_Init(ADC1, &ADC_REG_InitStruct);
  LL_ADC_DisableIT_EOC(ADC1);
  LL_ADC_DisableIT_EOS(ADC1);
  LL_ADC_DisableDeepPowerDown(ADC1);
  LL_ADC_EnableInternalRegulator(ADC1);
  ADC_CommonInitStruct.CommonClock = LL_ADC_CLOCK_ASYNC_DIV1;
  LL_ADC_CommonInit(__LL_ADC_COMMON_INSTANCE(ADC1), &ADC_CommonInitStruct);
  /** Configure Regular Channel 
  */
  LL_ADC_REG_SetSequencerRanks(ADC1, LL_ADC_REG_RANK_1, LL_ADC_CHANNEL_9);
  LL_ADC_SetChannelSamplingTime(ADC1, LL_ADC_CHANNEL_10, LL_ADC_SAMPLINGTIME_6CYCLES_5);
  LL_ADC_SetChannelSingleDiff(ADC1, LL_ADC_CHANNEL_10, LL_ADC_SINGLE_ENDED);
  LL_ADC_Enable(ADC1);//����ADC
}

/* USER CODE BEGIN 1 */
/***************************************************************************************
** ��������: Open_ADC
** ��������: ����ADC
** ��    ��: ADCx			ADC instance
** �� �� ֵ: None       
****************************************************************************************/
void Open_ADC(ADC_TypeDef *ADCx)
{
  __IO uint32_t wait_loop_index = 0;
	
  if (LL_ADC_IsEnabled(ADCx) == 0)//���ADC1�Ƿ���
  {
    LL_ADC_DisableDeepPowerDown(ADCx);//����ADC1��ϵ�ģʽ

    LL_ADC_EnableInternalRegulator(ADCx);//����ADC1�ڲ���ѹ������
    delay_us(20);
    LL_ADC_StartCalibration(ADCx, LL_ADC_SINGLE_ENDED);//����ADC�Զ�У��
    while (LL_ADC_IsCalibrationOnGoing(ADCx) != 0){}//�ȴ�ADCУ׼����
    delay_us(20);
    LL_ADC_Enable(ADCx);//����ADC
    while (LL_ADC_IsActiveFlag_ADRDY(ADCx) == 0){}//�ȴ�ADC׼����
  }
}
/***************************************************************************************
** ��������: Close_ADC
** ��������: �ر�ADC
** ��    ��: ADCx			ADC instance
** �� �� ֵ: None       
****************************************************************************************/
void Close_ADC(ADC_TypeDef *ADCx)
{
	LL_ADC_Disable(ADCx);//�ر�ADC
	LL_ADC_ClearFlag_ADRDY(ADCx);//���ADRDY��־
}
/***************************************************************************************
** ��������: ConversionStartPoll_ADC_GrpRegular
** ��������: ��ʼADC����ת��
** ��    ��: ADCx			ADC instance
** �� �� ֵ: None
****************************************************************************************/
void ConversionStartPoll_ADC_GrpRegular(ADC_TypeDef *ADCx)
{ 
  if ((LL_ADC_IsEnabled(ADCx) == 1)               &&
      (LL_ADC_IsDisableOngoing(ADCx) == 0)        &&
      (LL_ADC_REG_IsConversionOngoing(ADCx) == 0)   )
  {
    LL_ADC_REG_StartConversion(ADCx);//��ʼADCת��
  }
  else
  {
    //*�����޷�ִ��ADCת������*/
  }
  
  while (LL_ADC_IsActiveFlag_EOC(ADCx) == 0){}//�ȴ�ת�����
  LL_ADC_ClearFlag_EOC(ADCx);//���ת����ɱ�־
}
/***************************************************************************************
** ��������: Get_ADCVal
** ��������: ��ADCת�����ݼ����ѹֵ
** ��    ��: ADCx			ADC instance
** �� �� ֵ: None
****************************************************************************************/
uint16_t Get_ADCVal(ADC_TypeDef *ADCx)
{
	int adc_rst;
	ConversionStartPoll_ADC_GrpRegular(ADCx);//��ʼADCת��
	
	adc_rst = LL_ADC_REG_ReadConversionData12(ADCx);//��ȡADCת�����ݣ�12λ��

	return adc_rst;
}
extern uint16_t adc_rst_arry_test[10];
int get_ADC_RST(void)
{
	int adc_rst=0;
//	uint16 i =0;
//	for(i=0;i<99;i++)
//	{
//	LL_ADC_REG_StartConversion(ADC1);
//	while (LL_ADC_IsActiveFlag_EOC(ADC1) == 0);
//	adc_rst_arry_test[i]=LL_ADC_REG_ReadConversionData12(ADC1);	
//	}
		LL_ADC_REG_StartConversion(ADC1);
  	while (LL_ADC_IsActiveFlag_EOC(ADC1) == 0){};
  	adc_rst=LL_ADC_REG_ReadConversionData12(ADC1);	
	return adc_rst;
}

/* USER CODE END 1 */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
