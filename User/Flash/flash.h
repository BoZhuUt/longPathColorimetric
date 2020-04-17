#ifndef _FLASH_H
#define _FLASH_H
#include "main.h"
#include "stm32l4xx_hal_flash.h"
#include "embreg.h"

#define PAGE_SIZE						FLASH_PAGE_SIZE 													//Flash页大小2K
#define REG_GROUP_NUM				6																					//待存储的寄存器组数目
#define REG_GROUP_BYTES			104																				//寄存器组字节数
#define REG_STORAGE_OFFSET		(60 * 2048)//0x4000																		//modbus寄存器保存偏移地址

//Modbus寄存器组存储地址映射
#define REG_STORAGE_ADDR	  	(0x08000000+REG_STORAGE_OFFSET)						//modbus					寄存器组保存起始地址
#define SYS_STATUS_SADDR		(REG_STORAGE_ADDR + 0*REG_GROUP_BYTES)		//system_status		寄存器组存储位置起始地址
#define COM_SET_SADDR			(REG_STORAGE_ADDR + 1*REG_GROUP_BYTES)		//comm_settings		寄存器组存储位置起始地址
#define MEASURE_SET_SADDR		(REG_STORAGE_ADDR + 2*REG_GROUP_BYTES)		//measure_settings寄存器组存储位置起始地址
#define CAL_SET_SADDR			(REG_STORAGE_ADDR + 3*REG_GROUP_BYTES)		//calib_settings	寄存器组存储位置起始地址
#define FILTER_SET_SADDR		(REG_STORAGE_ADDR + 4*REG_GROUP_BYTES)		//filter_settings	寄存器组存储位置起始地址
#define DO_PARA_SADDR			(REG_STORAGE_ADDR + 5*REG_GROUP_BYTES)		//ph_orp_param		寄存器组存储位置起始地址

//部分STM32L431 Flash地址页映射关系图
#define ADDR_FLASH_PAGE_0     ((uint32_t)0x08000000) /* Base @ of Page 0, 2 Kbyte */
#define ADDR_FLASH_PAGE_1     ((uint32_t)0x08000800) /* Base @ of Page 1, 2 Kbyte */
#define ADDR_FLASH_PAGE_2     ((uint32_t)0x08001000) /* Base @ of Page 2, 2 Kbyte */
#define ADDR_FLASH_PAGE_3     ((uint32_t)0x08001800) /* Base @ of Page 3, 2 Kbyte */
#define ADDR_FLASH_PAGE_4     ((uint32_t)0x08002000) /* Base @ of Page 4, 2 Kbyte */
#define ADDR_FLASH_PAGE_5     ((uint32_t)0x08002800) /* Base @ of Page 5, 2 Kbyte */
#define ADDR_FLASH_PAGE_6     ((uint32_t)0x08003000) /* Base @ of Page 6, 2 Kbyte */
#define ADDR_FLASH_PAGE_7     ((uint32_t)0x08003800) /* Base @ of Page 7, 2 Kbyte */
#define ADDR_FLASH_PAGE_8     ((uint32_t)0x08004000) /* Base @ of Page 8, 2 Kbyte */
#define ADDR_FLASH_PAGE_9     ((uint32_t)0x08004800) /* Base @ of Page 9, 2 Kbyte */


HAL_StatusTypeDef StoreModbusRegs(void);
void ResetFlash(void);
void PowerOn_ReadModbusReg(void);


#endif
