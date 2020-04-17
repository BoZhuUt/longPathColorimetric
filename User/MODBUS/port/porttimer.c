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
  * @brief  TIM3初始化
  * @param  无
  * @retval 状态标志
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
  * @brief  开启TIM3
  * @param  无
  * @retval 无
  */
void vMBPortTimersEnable(void)
{
    /* Enable the timer with the timeout passed to xMBPortTimersInit( ) */
	
	//清除更新中断标志
	LL_TIM_ClearFlag_UPDATE(TIM6);
	//允许TIM3更新中断
	LL_TIM_EnableIT_UPDATE(TIM6);
	//将TIM3计数值清零
	LL_TIM_SetCounter(TIM6, 0);	
	//开启TIM3定时器

	LL_TIM_EnableCounter(TIM6);
}

/**
  * @brief  关闭TIM3
  * @param  无
  * @retval 无
  */
void vMBPortTimersDisable(void)
{
    /* Disable any pending timers. */
	
	//清除更新中断标志
	LL_TIM_ClearFlag_UPDATE(TIM6);	
	//禁止TIM3更新中断
	LL_TIM_DisableIT_UPDATE(TIM6);
	//将TIM3计数值清零
	LL_TIM_SetCounter(TIM6, 0);	
	//关闭TIM3定时器
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
  * @brief  TIM6中断函数
  * @param  无
  * @retval 无
  */
void TIM6_DAC_IRQHandler(void)
{
	//是否发送TIM6更新中断
	if (LL_TIM_IsActiveFlag_UPDATE(TIM6) != RESET)
	{
		//清除TIM6更新中断标志
		LL_TIM_ClearFlag_UPDATE(TIM6);

		prvvTIMERExpiredISR();
	}
}


