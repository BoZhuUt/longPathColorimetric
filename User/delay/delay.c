#include "delay.h"



void delay_us(uint32_t time_us)
{
	uint32_t i = 0,j =0;
	
	for(i = 0;i < time_us;i++)
	{
		for(j = 0;j < (RCC_Clocks.SYSCLK_Frequency / 18000000);j++)
		{
			__ASM("nop");
		}
	}
}

void delay_ms(uint32_t time_ms)
{
	uint32_t i = 0,j =0;
	
	for(i = 0;i < time_ms;i++)
	{
		for(j = 0;j < (RCC_Clocks.SYSCLK_Frequency / 18000);j++)
		{
			__ASM("nop");
		}
	}
}







