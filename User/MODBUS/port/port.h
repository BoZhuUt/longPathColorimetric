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
 * File: $Id: port.h,v 1.1 2006/08/22 21:35:13 wolti Exp $
 */

/* Define to prevent recursive inclusion -------------------------------------*/

#ifndef _PORT_H
#define _PORT_H

/* Includes ------------------------------------------------------------------*/

#include <assert.h>
#include <inttypes.h>
//#include "stm32f0xx.h"
#include "stm32l4xx.h"

/* Exported_Macros ------------------------------------------------------------*/
#define	INLINE                      inline
#define PR_BEGIN_EXTERN_C           extern "C" {
#define	PR_END_EXTERN_C             }

#define ENTER_CRITICAL_SECTION()  __set_PRIMASK(1) 
#define EXIT_CRITICAL_SECTION()   __set_PRIMASK(0) 

#ifndef TRUE
#define TRUE            1
#endif

#ifndef FALSE
#define FALSE           0
#endif

/* Exported_Types -------------------------------------------------------------*/
typedef uint8_t BOOL;

typedef unsigned char UCHAR;
typedef char CHAR;

typedef uint16_t USHORT;
typedef int16_t SHORT;

typedef uint32_t ULONG;
typedef int32_t LONG;

typedef uint8_t u8;
typedef uint16_t u16;
typedef uint32_t u32;

/* Exported_Functions ---------------------------------------------------------*/

#endif
/******************* (C) COPYRIGHT 2015 AndyChen *******END OF FILE*************/
