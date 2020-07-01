/**
  ******************************************************************************
  * @file    embreg.h
  * @author  AndyChen
  * @version V1.0
  * @date    2015-xx-xx
  * @brief   
  ******************************************************************************
  * @attention
  *
  *  
  ******************************************************************************
  */ 

/* Define to prevent recursive inclusion --------------------------------------*/

#ifndef __EMBREG_H

#define __EMBREG_H

/* Includes -------------------------------------------------------------------*/

#include "main.h"
#include "mb.h"
#include "mbutils.h"
#include "port.h"
//#include "ph.h"
#include "regmap.h"
//#include "app.h"

/* Exported_Macros ------------------------------------------------------------*/

/*----------------------ϵͳ��ؼĴ�����--------------------*/
#define SYSREG_REGNUM	9
//ϵͳ״̬�Ĵ�����ַ��־
#define SYSREG_FLAG		41	
//ϵͳ״̬�Ĵ�����ʼ��ַ
#define SYSREG_SADDR	(SYSREG_FLAG * 1000 + 1)
//ϵͳ״̬�Ĵ�������
#define SYSREG_NREGS	sizeof(SYS_STATUS_T)//(sizeof(SYS_STATUS_T)>>1)

#define COMSREG_REGNUM	4
//ͨ�����üĴ�����ַ��־
#define COMSREG_FLAG	42
//ͨ�����üĴ�����ʼ��ַ
#define COMSREG_SADDR	(COMSREG_FLAG * 1000 + 1)
//ͨ�����üĴ�������
#define COMSREG_NREGS	(sizeof(COMM_SETTINGS_T)>>1)

#define MSREG_REGNUM	4
//�������üĴ�����ַ��־
#define MSREG_FLAG		43
//�������üĴ�����ʼ��ַ
#define MSREG_SADDR		(MSREG_FLAG * 1000 + 1)	
//�������üĴ�������
#define MSREG_NREGS		(sizeof(MEASURE_SETTINGS_T)>>1)

#define CALSREG_REGNUM	3
//У׼���üĴ�����ַ��־
#define CALSREG_FLAG	44	
//У׼���üĴ�����ʼ��ַ
#define CALSREG_SADDR	(CALSREG_FLAG * 1000 + 1)		
//У׼���üĴ�������
#define CALSREG_NREGS	(sizeof(CALIB_SETTINGS_T)>>1)

#define FSREG_REGNUM	3
//�˲������üĴ�����ַ��־
#define FSREG_FLAG		45	
//�˲������üĴ�����ַ��־
#define FSREG_SADDR		(FSREG_FLAG * 1000 + 1)		
//�˲������üĴ�������
#define FSREG_NREGS		(sizeof(FILTER_SETTINGS_T)>>1)
	
#define MVREG_REGNUM	3
//����ֵ�Ĵ�����ַ��־
#define MVREG_FLAG		46	
//����ֵ�Ĵ�����ʼ��ַ
#define MVREG_SADDR		(MVREG_FLAG * 1000 + 1)			
//����ֵ�Ĵ�������
#define MVREG_NREGS		(sizeof(MEASURE_VALUES_T)>>1)
	
#define PHREG_REGNUM	6
//pH\ORPר�мĴ�����ַ��־
#define PHREG_FLAG		48		
//pH\ORPר�мĴ�����ʼ��ַ
#define PHREG_SADDR		(PHREG_FLAG * 1000 + 1)			
//pH\ORPר�мĴ�������
#define PHREG_NREGS		(sizeof(PH_ORP_PARAM_T)>>1)

/* Exported_Types -------------------------------------------------------------*/

/* Exported_Functions ---------------------------------------------------------*/

#endif
/******************* (C) COPYRIGHT 2015 AndyChen *******END OF FILE*************/
