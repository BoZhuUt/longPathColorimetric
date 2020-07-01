/**
  ******************************************************************************
  * @file    regmap.h
  * @author  AndyChen
  * @version V1.0
  * @date    2015-xx-xx
  * @brief   Modbus register address map
  ******************************************************************************
  * @attention
  *  
  ******************************************************************************
  */ 

/* Define to prevent recursive inclusion --------------------------------------*/

#ifndef __REGMAP_H

#define __REGMAP_H

/* Includes -------------------------------------------------------------------*/

#include "main.h"

/* Exported_Macros ------------------------------------------------------------*/

typedef uint16_t uint16;
typedef uint32_t uint32;

/* Exported_Types -------------------------------------------------------------*/

typedef enum
{
    IDLE=0,              
    CAL_ACID_SUCCESS,          /* 1, 酸性溶液校准成功 */
	  CAL_ALKALI_ORP_SUCCESS,    /* 2, pH:碱性溶液校准成功；ORP:orp校准成功 */
	  STD_VALUE_ERR,             /* 3, 未输入标样值或者输入的标样值不在校准范围 */
		RESULT_ERR,				         /* 4, 校准后的参数不合理 */
	  STD_TYPE_ERR,              /* 5, 标液值数据类型错误 */
	  CAL_PH7_SUCCESS            /* 6, 三点校准，原点，PH7校准成功 */
} CalibState;

//2 byte aligned
#pragma pack(2)

typedef	struct sysStatusOld
{											/* Register		Type  		 R/W */
	uint16		runStatus;					/* 41001	*/
	uint16   	commStatus;					/* 41002	*/
	uint16 		calibStatus;				/* 41003 	*/
	uint16		configStatus;				/* 41004	*/
	uint16		sensorType;					/* 41005	*/	
	uint32		serialNum;					/* 41006-41007	uint32 */
	uint16		softwareVer;				/* 41008	*/
	uint16		softwareRev;				/* 41009	*/
	uint16		hardwareVer;				/* 41010	*/
	uint16		reserved[4];				/* 41011-41014 reserved */
} SYS_STATUS_T_OLD;

typedef	struct sysStatus
{											/* Register		Type  		 R/W */
	uint16		runStatus;					/* 41001 */
	uint16   	commStatus;					/* 41002 */
	uint16 		calibStatus;				/* 41003 	 0，空闲；1，写pH的A点标液值成功；2，校准成功；3，标液值大小不合理；4，校准结果不合理；5，标液值非法浮点数 */
	uint16		configStatus;				/* 41004 */
	uint32    productNum;					/* 41005-41006	uint32 */
	char		deviceName[16];       /* 41007-41014	char[16] */
	char		serial[16];           /* 41015-41022	char[16] */
	char		hardwareVer[16];			/* 41023-41030  char[16] */
	char		softwareVer[16];      /* 41031-41038  char[16] */
	uint16  newStructFlg;					/* 41039 */
	uint16		reserved[13];				/* 41040-41052  reserved */
} SYS_STATUS_T;

typedef struct commSettings
{											/* Register		Type  		 R/W */
	uint16		modbusAddr;					/* 42001 	 */
	uint16		modbusDatabits;			/* 42002 	 */
	uint16		modbusParity;				/* 42003 	 */
	uint32		modbusBaud;					/* 42004-42005 */
	uint16		reserved[47];				/* 42006-42052  reserved */
} COMM_SETTINGS_T;

typedef	struct measureSettings
{											/* Register		Type  		 R/W */
	uint16		sampleCycle;				/* 43001	*/
	float		measureRange;			    /* 43002-43003	 float	 */
	float		smoothingFactor;		  /* 43004-43005 	 float   */
	uint16		command;				    /* 43006	*/
	float  pt100DriveCurrent;     /* 43007-43008   float   */
	uint16		reserved[44];			  /* 43009-43052   reserved */
} MEASURE_SETTINGS_T;

typedef	struct calibSettings
{											/* Register		Type  		 R/W */
	uint16		calibType;					/* 44001	      */
	float		  ORPCalibSolution;	  /* 44002-44003	float	*/
	float		  ORP_Slope;		      /* 44004-44005	float */
	float			temperatureR0;			/* 44006-44007  float */
	float 		temperatureRtda;		/* 44008-44009  float */
	float			ORP_E;							/* 44010-44011  float */
	float     pHSlopeFloor;       /* 44012-44013  float */
	float     pHSlopeCeiling;     /* 44014-44015  float */
	float     pHInterceptFloor;   /* 44016-44017  float */
	float     pHInterceptCeiling; /* 44018-44019  float */
	float     orpSlopeFloor;      /* 44020-44021  float */
	float     orpSlopeCeiling;    /* 44022-44023  reserved */
	uint16		reserved[29];				/* 44024-44052  reserved */
} CALIB_SETTINGS_T;

typedef	struct filterSettings
{											/* Register		Type  		 R/W */
	uint16		reserved[52];				/* 45001-45050   reserved */
} FILTER_SETTINGS_T;

typedef struct measureValues
{											/* Register		Type  		 R/W */
	uint32_t		sensorValue;				  /* 46001-46002	float  ph */
	uint32_t		sensorValue_mA;				/* 46003-46004	float */
	float   temperatureValue;			/* 46005-46006	float */
	float   sensorValue1;         /* 46007-46008  float  orp, V */
	float		sensorValue_mA1;			/* 46009-46010  float */
	float 	pt100Res;							/* 46011-46012  float*/
	float   orp_mV;								/* 46013-46014	float  orp, mV */	
	uint16  reserved[38];         /* 46015-46052  reserved */
} MEASURE_VALUES_T;


// Below are sensor specific registers, pH meter may have different definition
// All sensor specific registers start from 48001

typedef	struct pH_ORP_Param
{											          /* Register		Type  		     R/W  */	
	uint16    startNum;           /* 48001   16bit integer   r/w */
	uint16    ct365;              /* 48002   16bit integer   r/w */
	uint16    t365Gain;           /* 48003   16bit integer   r/w */
	uint16    dark;               /* 48004   16bit integer   r/w */
	uint16    t365;               /* 48005   16bit integer   r/w */
	uint16    ct410;              /* 48006   16bit integer   r/w */
	uint16    t410Gain;           /* 48007   16bit integer   r/w */
	uint16    t410dark;           /* 48008   16bit integer   r/w */
	uint16    t410;               /* 48009   16bit integer   r/w */
	uint16		reserved[43];				/* 480010-48052   reserved */	
} PH_ORP_PARAM_T;

#pragma pack()

/* Exported_Functions ---------------------------------------------------------*/


#endif
/******************* (C) COPYRIGHT 2015 AndyChen *******END OF FILE*************/
