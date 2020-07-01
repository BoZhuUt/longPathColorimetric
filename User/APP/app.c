#include "app.h"

void ParaInit(void)
{
	memset(system_status.deviceName,0,sizeof(system_status.deviceName));
	memset(system_status.serial,0,sizeof(system_status.serial));
	memset(system_status.hardwareVer,0,sizeof(system_status.hardwareVer));
	memset(system_status.softwareVer,0,sizeof(system_status.softwareVer));
	system_status.runStatus=0;
	system_status.commStatus=0;
	system_status.calibStatus=IDLE;
	system_status.configStatus=1;
	strcpy(system_status.serial,"0");
	strcpy(system_status.hardwareVer,HW_VERSION);
	strcpy(system_status.softwareVer,SW_VERSION);
	system_status.newStructFlg=0X55;
	system_status.productNum=12345;
	memset(system_status.reserved,0,sizeof(system_status.reserved));	
		
	comm_settings.modbusAddr = 45;
	comm_settings.modbusDatabits=8;				
	comm_settings.modbusParity=MODBUS_PARITY_EVEN;
	comm_settings.modbusBaud=9600;
	memset(comm_settings.reserved,0,sizeof(comm_settings.reserved));
		
		
		
	measure_settings.command=0;
	measure_settings.sampleCycle=4;
	memset(measure_settings.reserved,0,sizeof(measure_settings.reserved));	
		
	memset(calib_settings.reserved,0,sizeof(calib_settings.reserved));

	ph_orp_param.ct365=1099;
	ph_orp_param.t365Gain=0;
	ph_orp_param.ct410=1;
	ph_orp_param.t410Gain=0;
	memset(ph_orp_param.reserved,0,sizeof(ph_orp_param.reserved));
	
	__disable_irq();
	StoreModbusRegs();
	__enable_irq();
}