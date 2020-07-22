#include "measure.h"
#include "adc.h"

float s365dRaw;
float s365dRawPrev = -1.0;

float s470dRaw;
float s470dRawPrev = -1.0;
uint8_t channnel_select_flag;
/*turnon led365*/
void turn_on_led_d7(void)
{
	LED_SWITCH_A0_LOW();
	LED_SWITCH_A1_LOW();
}
void turn_on_led_d8(void)
{
	LED_SWITCH_A0_HIGH();
	LED_SWITCH_A1_LOW();
}
void turn_on_led_d9(void)
{
	LED_SWITCH_A0_LOW();
	LED_SWITCH_A1_HIGH();
}
/*turnon led405*/
void turn_on_led_d10(void)
{
	LED_SWITCH_A0_HIGH();
	LED_SWITCH_A1_HIGH();
}
/*turnoff led*/
void turn_off_led(void)
{
	LED_SWITCH_A0_LOW();
	LED_SWITCH_A1_LOW();
}
/**/
void pd1_adc(void)
{
}
/**/
void pd2_adc(void)
{
}
/**/
void pd3_adc(void)
{
}
#define aveNum 100
float adcBuffer;
float getAD_result(void)
{
    u16 ad_arry[4];
	u16 i,j,ad_temp;
	float aveRst=0.0;	 
	for(j=0;j<aveNum;j++)
	{  	
		ad_temp=get_ADC_RST(); 	
		aveRst+=ad_temp;
	}
			
	 aveRst/=aveNum;
	 return aveRst;
}
float channel(u8 meaChannel)
{
	return getAD_result();
}
