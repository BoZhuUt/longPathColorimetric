#include "flash.h"
#include "pga113.h"
//#include "modbus.h"
//#include "portrw.h"
typedef struct
{
	void 			*pData;		//指向modbus相关寄存器的数据指针
	uint32_t	addr;			//寄存器组Flash存储地址
	uint16_t 	nbyte;		//寄存器组字节数
}REG_STORE_TypeDef;

REG_STORE_TypeDef RegsStoreInfArr[REG_GROUP_NUM] =	//存入待存储寄存器的首地址,Flash地址和寄存器个数
{
	{&system_status		,SYS_STATUS_SADDR	,SYSREG_NREGS	},
	{&comm_settings		,COM_SET_SADDR		,COMSREG_NREGS	},	
	{&measure_settings	,MEASURE_SET_SADDR	,MSREG_NREGS	},
	{&calib_settings	,CAL_SET_SADDR		,CALSREG_NREGS	},
	{&filter_settings	,FILTER_SET_SADDR	,FSREG_NREGS	},
	{&ph_orp_param		,DO_PARA_SADDR		,PHREG_NREGS	}		
};

/**
  * @brief  Flash擦除
  * @param  地址
  * @retval 执行状态
  */
HAL_StatusTypeDef STMFLASH_Erase(uint32_t e_addr)
{
	uint32_t PageError = 0;
	FLASH_EraseInitTypeDef pEraseInit;
	HAL_StatusTypeDef status = HAL_OK;
	
//	HAL_FLASH_Unlock();					
	
		/* Clear OPTVERR bit set on virgin samples */
  __HAL_FLASH_CLEAR_FLAG(FLASH_FLAG_OPTVERR); //This statement is very important, otherwise the first erasure is invalid
	
	pEraseInit.Banks = FLASH_BANK_1;	//擦除Bank1
	pEraseInit.NbPages = 1;						//擦除扇区的个数
	pEraseInit.Page = REG_STORAGE_OFFSET / PAGE_SIZE;//擦除Page8
	pEraseInit.TypeErase = FLASH_TYPEERASE_PAGES;//擦除类型Page
	status = HAL_FLASHEx_Erase(&pEraseInit, &PageError);
	
//	HAL_FLASH_Lock();
	
	return status;
}

/**
  * @brief  保存数据到stm32内部flash
	* @param  pdata:数据指针
	*					nbyte:写入字节数
  * @retval 执行状态
  */
static HAL_StatusTypeDef STMFLASH_Write(void *pdata,uint32_t addr,uint8_t nbyte)
{
	HAL_StatusTypeDef status=HAL_OK;
	uint64_t *pd=pdata;
//	HAL_FLASH_Unlock();
	
	//写入数据
	for(uint8_t i=0;i<(nbyte / 8) && status==HAL_OK;i++)
	{
		status = HAL_FLASH_Program(FLASH_TYPEPROGRAM_DOUBLEWORD, addr, *pd);		
		addr += 8;
		pd++;
	}		
	
//	HAL_FLASH_Lock();	
	return status;
}

/**
  * @brief  存储modbus寄存器
  * @param  无
  * @retval 执行状态
  */
HAL_StatusTypeDef StoreModbusRegs(void)
{
	HAL_StatusTypeDef s_status;
	
	HAL_FLASH_Unlock();
	
	//擦除flash
	s_status = STMFLASH_Erase(REG_STORAGE_ADDR);
	if(s_status != HAL_OK)
		return s_status;
	
	//保存寄存器到flash
	for(uint8_t i=0 ; i < REG_GROUP_NUM ; i++ )
	{
		s_status = STMFLASH_Write(RegsStoreInfArr[i].pData,RegsStoreInfArr[i].addr ,RegsStoreInfArr[i].nbyte);
		if(s_status != HAL_OK)
			return s_status;
	}
	
	HAL_FLASH_Lock();
	
	return s_status;
}

void ResetFlash(void)
{
//	/*system_status结构体初始化*/														
//	memset(system_status.deviceName,0,sizeof(system_status.deviceName));	//设备名称
//	memset(system_status.serial,0,sizeof(system_status.serial));					//SN
//	memset(system_status.hardwareVer,0,sizeof(system_status.hardwareVer));//硬件版本号
//	memset(system_status.softwareVer,0,sizeof(system_status.softwareVer));//软件版本号			
//	system_status.runStatus = 0;
//	system_status.commStatus = 0;
//	system_status.calibStatus = 0;
//	system_status.configStatus = 0;
//	system_status.productNum = PRODUCT_NUM;						//设备编号PN
//	strcpy(system_status.deviceName	,DEVICE_NAME);		//设备名称
//	strcpy(system_status.serial			,SERIAL_NUM);			//序列号SN
//	strcpy(system_status.hardwareVer,HW_VERSION_NUM);	//硬件版本号
//	strcpy(system_status.softwareVer,SW_VERSION_NUM);	//软件版本号		
//	system_status.newStructFlg = 0x2222;
//	memset(system_status.reserved,0,sizeof(system_status.reserved));	
//		
//	/*comm_settings结构体初始化*/
//	comm_settings.modbusAddr = 1;
//	comm_settings.modbusDatabits=9;		//包含校验位		
//	comm_settings.modbusParity = MB_PAR_EVEN;
//	comm_settings.modbusBaud = 9600;
//	memset(comm_settings.reserved,0,sizeof(comm_settings.reserved));
//		
//	/*measure_settings结构体初始化*/
//	measure_settings.sampleCycle = 0;
//	measure_settings.measureRange = 0;
//	measure_settings.smoothingFactor = 0;
//	measure_settings.command = 0;
//	measure_settings.DODriveCurrent = 0;
//	measure_settings.RedCurrent  = 16;		
//	measure_settings.BlueCurrent = 10;	
//	measure_settings.PhaseAtanChange = 0;
//	measure_settings.SingleSinDot = SingleSinDot256;
//	measure_settings.SkipSineNum = 10;
//	measure_settings.StartPhase = Sin_270;
//	measure_settings.PollTimerControl = 1;
//	measure_settings.PollTimerNum = 60000;
//	measure_settings.RedSlipFilterNum = 3;
//	measure_settings.BlueSlipFilterNum = 3;
//	measure_settings.AD7792_SlipFilterNum = 3;
//	measure_settings.MEDIAN_FILTER_NUM = 3;
//	memset(measure_settings.reserved,0,sizeof(measure_settings.reserved));

//	/*calib_settings结构体初始化*/
//	calib_settings.calibType = 0;
//	calib_settings.Unix_Time_Stamp = 0;
//	calib_settings.Air_Calib = 0;
//	calib_settings.Zero_Calib = 0;
//	calib_settings.Pres_Calib = 0.0;
//	calib_settings.RTD_A = 3.908E-3;
//	calib_settings.RTD_B = -5.8019E-7;
//	calib_settings.RTD_R0= 100;
//	calib_settings.KT0 = 7.160298;
//	calib_settings.KT1 = -0.050687;
//	calib_settings.taoPDT0 = 19.29937;
//	calib_settings.taoPDT1 = -0.079695;
//	calib_settings.tao0T0  = 66.41589;
//	calib_settings.tao0T1  = -0.201875;
//	calib_settings.Humi = 1.0;
//	memset(calib_settings.reserved,0,sizeof(calib_settings.reserved));
//	
//	/*do_param结构体初始化*/
//	do_param.PO1 = 0.0;
//	do_param.T1  = 0.0;
//	do_param.PO2 = 0.0;
//	do_param.T2  = 0.0;
//	do_param.PO3 = 0.0;
//	do_param.T3  = 0.0;
//	do_param.Conductivity = 0.0;
//	do_param.Attitude = 0.0;
//	do_param.Phase_Correction1 = 0.0;
//	do_param.Phase_Correction2 = 0.0;
//	do_param.Phase_Correction3 = 0.0;
//	do_param.FreqSet		 = Freq1220;
//	do_param.BlueGainSet = PGA_GAIN_5;
//	do_param.RedGainSet  = PGA_GAIN_20;
//	memset(do_param.reserved,0,sizeof(do_param.reserved));
//		
//	__disable_irq();
//	StoreModbusRegs();
//	__enable_irq();
}

void PowerOn_ReadModbusReg(void)
{
	system_status     = *((__IO SYS_STATUS_T *)SYS_STATUS_SADDR);
	comm_settings     = *((__IO COMM_SETTINGS_T *)COM_SET_SADDR);	
	measure_settings  = *((__IO MEASURE_SETTINGS_T *)MEASURE_SET_SADDR);
	calib_settings    = *((__IO CALIB_SETTINGS_T *)CAL_SET_SADDR);	
	filter_settings   = *((__IO FILTER_SETTINGS_T *)FILTER_SET_SADDR);
	ph_orp_param          = *((__IO PH_ORP_PARAM_T *)DO_PARA_SADDR);
//	if(system_status.newStructFlg != 0x2222)   //从未下载过程序的全新探头
//	{
//		ResetFlash();
//	}
//	else
//	{
//		if(strcmp(system_status.softwareVer,SW_VERSION_NUM) != 0 )
//		{
//			memset(system_status.softwareVer,0,sizeof(system_status.softwareVer));
//			strcpy(system_status.softwareVer,SW_VERSION_NUM);
//			__disable_irq();
//			StoreModbusRegs();
//			__enable_irq();			
//		}
//		if(measure_settings.command != 0)
//		{
//			measure_settings.command = 0;
//			__disable_irq();
//			StoreModbusRegs();
//			__enable_irq();	
//		}
//		if(measure_settings.SingleSinDot == 0)
//		{
//			measure_settings.SingleSinDot = SingleSinDot256;
//			__disable_irq();
//			StoreModbusRegs();
//			__enable_irq();	
//		}
//	}
}




