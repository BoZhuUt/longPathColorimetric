/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; Copyright (c) 2020 STMicroelectronics.
  * All rights reserved.</center></h2>
  *
  * This software component is licensed by ST under BSD 3-Clause license,
  * the "License"; You may not use this file except in compliance with the
  * License. You may obtain a copy of the License at:
  *                        opensource.org/licenses/BSD-3-Clause
  *
  ******************************************************************************
  */
/* USER CODE END Header */

/* Includes ------------------------------------------------------------------*/
#include "main.h"
#include "adc.h"
#include "tim.h"
#include "gpio.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

#include "port.h"
#include "mb.h"
#include "measure.h"
#include "stdio.h"
/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */
LL_RCC_ClocksTypeDef RCC_Clocks;

MEASURE_VALUES_T measure_values;
SYS_STATUS_T system_status;
MEASURE_SETTINGS_T measure_settings;
PH_ORP_PARAM_T ph_orp_param;
COMM_SETTINGS_T  comm_settings;
//PH_ORP_PARAM_T  pH_ORP_Param;
CALIB_SETTINGS_T calib_settings;
FILTER_SETTINGS_T filter_settings;
uint8_t flag = 0,flag1 = 0;
uint16_t adc_rst_arry_test[10];
uint8_t MEASURE_FLAG=0;
/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */
/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/

/* USER CODE BEGIN PV */

/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
/* USER CODE BEGIN PFP */
void USART_SEND_DATA(uint8_t DATA);
void UART1_SEND_CHAR(char *str);
int fputc(int ch,FILE *f);
/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */
char adc_arry[10];
/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{
  /* USER CODE BEGIN 1 */
	SCB->VTOR = FLASH_BASE | 0x05000;				//设置APP启动地址
	int adc_result,i,j;
	 float adc_ave=0;
  /* USER CODE END 1 */

  /* MCU Configuration--------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
  

  LL_APB2_GRP1_EnableClock(LL_APB2_GRP1_PERIPH_SYSCFG);
  LL_APB1_GRP1_EnableClock(LL_APB1_GRP1_PERIPH_PWR);

  NVIC_SetPriorityGrouping(NVIC_PRIORITYGROUP_4);

  /* System interrupt init*/

  /* USER CODE BEGIN Init */

  /* USER CODE END Init */

  /* Configure the system clock */
  SystemClock_Config();

  /* USER CODE BEGIN SysInit */
	LL_RCC_GetSystemClocksFreq(&RCC_Clocks);
  /* USER CODE END SysInit */

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_ADC1_Init();
  MX_TIM7_Init();
  /* USER CODE BEGIN 2 */
	PowerOn_ReadModbusReg();
  LTC2630ISC6_init();
	LL_TIM_TIM7_ENABLE();

  //PowerOn_ReadModbusReg();
  eMBInit(MB_RTU,45,0,9600,MB_PAR_EVEN);

	/*Enable Modbus protocol stack*/
	eMBEnable();
	measure_values.sensorValue = 20;
	measure_values.sensorValue_mA = 0;
	//RS485_SEND_EN();
	configPGA113(ch0,1);
	//4~20mA
		write_to_LTC2630ISC6(LTC2630ISC6_WRITE_TO_AND_UPDATE,3000);
		//AD5410_IOUT(8,16);
  /* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
  while (1)
  {
		
    /* USER CODE END WHILE */

    /* USER CODE BEGIN 3 */
	  eMBPoll();
		if(ph_orp_param.startNum==1)
		{
			ph_orp_param.startNum=0;
			StoreModbusRegs();
		}
		if(MEASURE_FLAG>3)
		{
			LL_USART_Disable(USART1);
			configPGA113(ch0,ph_orp_param.t365Gain);
			Open_ADC(ADC1);
			ph_orp_param.dark=getAD_result();
			turn_on_led_d10();
			write_to_LTC2630ISC6(LTC2630ISC6_WRITE_TO_AND_UPDATE,ph_orp_param.ct365);
			delay_ms(50);
			Open_ADC(ADC1);
			ph_orp_param.t365=getAD_result()-ph_orp_param.dark;
			write_to_LTC2630ISC6(LTC2630ISC6_WRITE_TO_AND_UPDATE,0);
			MEASURE_FLAG=0;
			LL_USART_Enable(USART1);
		}		
	//	ITM_SendChar(0x55);     //没接JTDO 无法使用ITM调试
  }
  /* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
  LL_FLASH_SetLatency(LL_FLASH_LATENCY_4);

  if(LL_FLASH_GetLatency() != LL_FLASH_LATENCY_4)
  {
  Error_Handler();  
  }
  LL_PWR_SetRegulVoltageScaling(LL_PWR_REGU_VOLTAGE_SCALE1);
  LL_RCC_HSE_Enable();

   /* Wait till HSE is ready */
  while(LL_RCC_HSE_IsReady() != 1)
  {
    
  }
  LL_RCC_PLL_ConfigDomain_SYS(LL_RCC_PLLSOURCE_HSE, LL_RCC_PLLM_DIV_1, 20, LL_RCC_PLLR_DIV_2);
  LL_RCC_PLL_EnableDomain_SYS();
  LL_RCC_PLL_Enable();

   /* Wait till PLL is ready */
  while(LL_RCC_PLL_IsReady() != 1)
  {
    
  }
  LL_RCC_PLLSAI1_ConfigDomain_ADC(LL_RCC_PLLSOURCE_HSE, LL_RCC_PLLM_DIV_1, 8, LL_RCC_PLLSAI1R_DIV_2);
  LL_RCC_PLLSAI1_EnableDomain_ADC();
  LL_RCC_PLLSAI1_Enable();

   /* Wait till PLLSAI1 is ready */
  while(LL_RCC_PLLSAI1_IsReady() != 1)
  {
    
  }
  LL_RCC_SetSysClkSource(LL_RCC_SYS_CLKSOURCE_PLL);

   /* Wait till System clock is ready */
  while(LL_RCC_GetSysClkSource() != LL_RCC_SYS_CLKSOURCE_STATUS_PLL)
  {
  
  }
  LL_RCC_SetAHBPrescaler(LL_RCC_SYSCLK_DIV_1);
  LL_RCC_SetAPB1Prescaler(LL_RCC_APB1_DIV_1);
  LL_RCC_SetAPB2Prescaler(LL_RCC_APB2_DIV_1);

  LL_Init1msTick(80000000);

  LL_SetSystemCoreClock(80000000);
  LL_RCC_SetADCClockSource(LL_RCC_ADC_CLKSOURCE_PLLSAI1);
}

/* USER CODE BEGIN 4 */

int fputc(int ch, FILE *f) 
{
    return(ch);
}
void USART_SEND_DATA(uint8_t DATA)
{
	RS485_SEND_EN();
	LL_USART_TransmitData8(USART1,DATA);
	while(LL_USART_IsActiveFlag_TC(USART1) == RESET)
	{
		;
	}
	
}
void UART1_SEND_CHAR(char *str)
{
	while(*str)
	{ 
		LL_USART_TransmitData8(USART1,*str);
		while(LL_USART_IsActiveFlag_TC(USART1) == RESET)
		{
			;
		}
		//成只发送最后一个字符（覆盖）
		str++;
  }
	}

	void measure_tim7_IRQ()
{
	 MEASURE_FLAG++;
	if(MEASURE_FLAG==10)
	{MEASURE_FLAG=0;}
	 LL_TIM_ClearFlag_UPDATE(TIM7);
}
/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */

  /* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t *file, uint32_t line)
{ 
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
     tex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
