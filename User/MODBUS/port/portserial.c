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
 * File: $Id: portserial.c,v 1.1 2006/08/22 21:35:13 wolti Exp $
 */

#include "port.h"

/* ----------------------- Modbus includes ----------------------------------*/
#include "mb.h"
#include "mbport.h"

#include "main.h"

/* ----------------------- Private Macros -----------------------------------*/

/*控制端口宏定义*/
#define RS485_IO_CTRL_PORT	GPIOB
#define RS485_IO_CTRL_PIN	GPIO_Pin_12

#define RS485_LED_CTRL_PORT	GPIOB
#define RS485_LED_CTRL_PINY	GPIO_Pin_8	
#define RS485_LED_CTRL_PING	GPIO_Pin_9

/*通信只是灯端口及引脚定义*/
#define RS485_GLED_ON	GPIO_SetBits(GPIOB,GPIO_Pin_8)
#define RS485_GLED_OFF	GPIO_ResetBits(GPIOB,GPIO_Pin_8)

#define RS485_YLED_ON	GPIO_SetBits(GPIOB,GPIO_Pin_9)
#define RS485_YLED_OFF	GPIO_ResetBits(GPIOB,GPIO_Pin_9)

/* ----------------------- static functions ---------------------------------*/
static void prvvUARTTxReadyISR(void);
static void prvvUARTRxISR(void);

/* ----------------------- Start implementation -----------------------------*/

/**
  * @brief  串口收发开关函数
  * @param  无
  * @retval 无
  */
void vMBPortSerialEnable( BOOL xRxEnable, BOOL xTxEnable )
{
    /* If xRXEnable enable serial receive interrupts. If xTxENable enable
     * transmitter empty interrupts.
     */	
	/*------------------串口接收中断控制--------------------*/
	if(xRxEnable==TRUE)
	{
		//允许串口接收中断
		LL_USART_EnableIT_RXNE(USART1);
		
		//RS485接收
		RS485_RECEIVE_EN();
	}
	else
	{
		//禁止串口接收中断
		LL_USART_DisableIT_RXNE(USART1);
		
		//RS485发送
		RS485_SEND_EN();
	}		
	
	/*------------------串口发送中断控制--------------------*/
	if(xTxEnable==TRUE)
	{
		//允许串口发送中断
		LL_USART_EnableIT_TXE(USART1);
		
		//RS485发送
		RS485_SEND_EN();
	}
	else
	{
		//禁止串口发送中断
		LL_USART_DisableIT_TXE(USART1);
		
		//RS485接收
		RS485_RECEIVE_EN();
	}
}

/**
  * @brief  串口初始化函数
  * @param  无
  * @retval 状态标志
  */
BOOL xMBPortSerialInit( UCHAR ucPORT, ULONG ulBaudRate, UCHAR ucDataBits, eMBParity eParity )
{
	MX_USART1_Modbus_Init(ulBaudRate,eParity);	
	
    return TRUE;
}

/**
  * @brief  输出数据函数
  * @param  无
  * @retval 无
  */
BOOL xMBPortSerialPutByte( CHAR ucByte )
{	
    /* Put a byte in the UARTs transmit buffer. This function is called
     * by the protocol stack if pxMBFrameCBTransmitterEmpty( ) has been
     * called. */
//	static uint8_t m=0;

	LL_USART_TransmitData8(USART1, ucByte);
	
//	//取反通信指示灯
//	if(++m%2)
//		RS485_YLED_ON;
//	else
//		RS485_YLED_OFF;
//	
	//等待发送完成

	while(LL_USART_IsActiveFlag_TC(USART1) == RESET)
	{
		;
	}
    return TRUE;
}

/**
  * @brief  接收数据函数
  * @param  无
  * @retval 完成状态
  */
BOOL xMBPortSerialGetByte( CHAR * pucByte )
{
//	static uint8_t n=0;
    /* Return the byte in the UARTs receive buffer. This function is called
     * by the protocol stack after pxMBFrameCBByteReceived( ) has been called.
     */	

	*pucByte = LL_USART_ReceiveData8(USART1);
//	//取反通信指示灯
//	if(++n%2)
//		RS485_YLED_ON;
//	else
//		RS485_YLED_OFF;
	
    return TRUE;
}

/* Create an interrupt handler for the transmit buffer empty interrupt
 * (or an equivalent) for your target processor. This function should then
 * call pxMBFrameCBTransmitterEmpty( ) which tells the protocol stack that
 * a new character can be sent. The protocol stack will then call 
 * xMBPortSerialPutByte( ) to send the character.
 */
static void prvvUARTTxReadyISR(void)
{
    pxMBFrameCBTransmitterEmpty();
}

/* Create an interrupt handler for the receive interrupt for your target
 * processor. This function should then call pxMBFrameCBByteReceived( ). The
 * protocol stack will then call xMBPortSerialGetByte( ) to retrieve the
 * character.
 */
static void prvvUARTRxISR(void)
{
    pxMBFrameCBByteReceived();  //xMBRTUReceiveFSM
}

/**
  * @brief  串口中断函数
  * @param  无
  * @retval 无
 */
void USART1_IRQHandler(void)
{
	//	//接收中断
	if(LL_USART_IsActiveFlag_RXNE(USART1) !=RESET)
	{
		prvvUARTRxISR(); 
		//清除中断标志
	}

	//发送寄存器空中断TXE
	if(LL_USART_IsActiveFlag_TXE(USART1) != RESET)
	{
		prvvUARTTxReadyISR();
		//清除中断标志
//		LL_USART_ClearFlag_TC(USART1);
	}
	//Clear Receiver Time Out Flag
	if(LL_USART_IsActiveFlag_RTO(USART1) != RESET)
	{
		LL_USART_ClearFlag_RTO(USART1);
	}
	
}
//void USART2_IRQHandler(void)
//{
//	IWDG_ReloadCounter();//喂狗
//	//接收中断
//	if(USART_GetITStatus(USART2, USART_IT_RXNE)!=RESET)
//	{
//		prvvUARTRxISR(); 
//		//清除中断标志
//		USART_ClearITPendingBit(USART2, USART_IT_RXNE); 
//	}

//	//发送寄存器空中断
//	if(USART_GetITStatus(USART2, USART_IT_TXE)!=RESET)
//	{
//		prvvUARTTxReadyISR();
//		//清除中断标志
//		USART_ClearITPendingBit(USART2, USART_IT_TXE);
//	}
//	
//	if(USART_GetFlagStatus(USART2,USART_FLAG_ORE)!=RESET)
//	{
//		USART_ClearFlag(USART2,USART_FLAG_ORE);  //清除溢出标志
//	}
//	
//  if(USART_GetFlagStatus(USART2,USART_FLAG_NE)!=RESET)
//	{
//		USART_ClearFlag(USART2,USART_FLAG_NE);  //清除噪声错误标志
//	}
//	
//	if(USART_GetFlagStatus(USART2,USART_FLAG_FE)!=RESET)
//	{
//		USART_ClearFlag(USART2,USART_FLAG_FE);   //清除帧错误标志
//	}
//	
//	if(USART_GetFlagStatus(USART2,USART_FLAG_PE)!=RESET)
//	{
//		USART_ClearFlag(USART2,USART_FLAG_PE);   //清除校验错误标志
//	}
//}


