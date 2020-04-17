/*
 * FreeModbus Libary: BARE Port
 * Copyright (C) 2006 Christian Walter <wolti@sil.at>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 *
 * File: $Id: porttimer.c,v 1.1 2006/08/22 21:35:13 wolti Exp $
 */

/* ----------------------- Platform includes --------------------------------*/
#include "port.h"

/* ----------------------- Modbus includes ----------------------------------*/
#include "mb.h"
#include "mbport.h"

#include "main.h"
/* ----------------------- static functions ---------------------------------*/
static void prvvTIMERExpiredISR(void);

/* ----------------------- Start implementation -----------------------------*/

/**
  * @brief  TIM3��ʼ��
  * @param  ��
  * @retval ״̬��־
  */
BOOL xMBPortTimersInit( USHORT usTim1Timerout50us )
{
	
	MX_TIM6_Modbus_Init(((RCC_Clocks.PCLK1_Frequency / 20000) - 1),usTim1Timerout50us);
	LL_TIM_ClearFlag_UPDATE(TIM6);
	LL_TIM_EnableIT_UPDATE(TIM6);
	
	LL_TIM_DisableCounter(TIM6);
	
    return TRUE;
}

/**
  * @brief  ����TIM3
  * @param  ��
  * @retval ��
  */
void vMBPortTimersEnable(void)
{
    /* Enable the timer with the timeout passed to xMBPortTimersInit( ) */
	
	//��������жϱ�־
	LL_TIM_ClearFlag_UPDATE(TIM6);
	//����TIM3�����ж�
	LL_TIM_EnableIT_UPDATE(TIM6);
	//��TIM3����ֵ����
	LL_TIM_SetCounter(TIM6, 0);	
	//����TIM3��ʱ��

	LL_TIM_EnableCounter(TIM6);
}

/**
  * @brief  �ر�TIM3
  * @param  ��
  * @retval ��
  */
void vMBPortTimersDisable(void)
{
    /* Disable any pending timers. */
	
	//��������жϱ�־
	LL_TIM_ClearFlag_UPDATE(TIM6);	
	//��ֹTIM3�����ж�
	LL_TIM_DisableIT_UPDATE(TIM6);
	//��TIM3����ֵ����
	LL_TIM_SetCounter(TIM6, 0);	
	//�ر�TIM3��ʱ��
	LL_TIM_DisableCounter(TIM6);
}

/* Create an ISR which is called whenever the timer has expired. This function
 * must then call pxMBPortCBTimerExpired( ) to notify the protocol stack that
 * the timer has expired.
 */
static void prvvTIMERExpiredISR(void)
{
    (void)pxMBPortCBTimerExpired();
}

/**
  * @brief  TIM6�жϺ���
  * @param  ��
  * @retval ��
  */
void TIM6_DAC_IRQHandler(void)
{
	//�Ƿ���TIM6�����ж�
	if (LL_TIM_IsActiveFlag_UPDATE(TIM6) != RESET)
	{
		//���TIM6�����жϱ�־
		LL_TIM_ClearFlag_UPDATE(TIM6);

		prvvTIMERExpiredISR();
	}
}


