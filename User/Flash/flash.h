#ifndef _FLASH_H
#define _FLASH_H
#include "main.h"
#include "stm32l4xx_hal_flash.h"
#include "embreg.h"

#define PAGE_SIZE						FLASH_PAGE_SIZE 													//Flashҳ��С2K
#define REG_GROUP_NUM				6																					//���洢�ļĴ�������Ŀ
#define REG_GROUP_BYTES			104																				//�Ĵ������ֽ���
#define REG_STORAGE_OFFSET		(60 * 2048)//0x4000																		//modbus�Ĵ�������ƫ�Ƶ�ַ

//Modbus�Ĵ�����洢��ַӳ��
#define REG_STORAGE_ADDR	  	(0x08000000+REG_STORAGE_OFFSET)						//modbus					�Ĵ����鱣����ʼ��ַ
#define SYS_STATUS_SADDR		(REG_STORAGE_ADDR + 0*REG_GROUP_BYTES)		//system_status		�Ĵ�����洢λ����ʼ��ַ
#define COM_SET_SADDR			(REG_STORAGE_ADDR + 1*REG_GROUP_BYTES)		//comm_settings		�Ĵ�����洢λ����ʼ��ַ
#define MEASURE_SET_SADDR		(REG_STORAGE_ADDR + 2*REG_GROUP_BYTES)		//measure_settings�Ĵ�����洢λ����ʼ��ַ
#define CAL_SET_SADDR			(REG_STORAGE_ADDR + 3*REG_GROUP_BYTES)		//calib_settings	�Ĵ�����洢λ����ʼ��ַ
#define FILTER_SET_SADDR		(REG_STORAGE_ADDR + 4*REG_GROUP_BYTES)		//filter_settings	�Ĵ�����洢λ����ʼ��ַ
#define DO_PARA_SADDR			(REG_STORAGE_ADDR + 5*REG_GROUP_BYTES)		//ph_orp_param		�Ĵ�����洢λ����ʼ��ַ

//����STM32L431 Flash��ַҳӳ���ϵͼ
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
