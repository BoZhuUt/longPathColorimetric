#include "measure.h"
#include "adc.h"

float s365dRaw;
float s365dRawPrev = -1.0;

float s470dRaw;
float s470dRawPrev = -1.0;
uint8_t channnel_select_flag;
/*turnon led365*/
void turn_on_led_d7()
{
	LED_SWITCH_A0_LOW();
	LED_SWITCH_A1_LOW();
}
void turn_on_led_d8()
{
	LED_SWITCH_A0_HIGH();
	LED_SWITCH_A1_LOW();
}
void turn_on_led_d9()
{
	LED_SWITCH_A0_LOW();
	LED_SWITCH_A1_HIGH();
}
/*turnon led405*/
void turn_on_led_d10()
{
	LED_SWITCH_A0_HIGH();
	LED_SWITCH_A1_HIGH();
}
/*turnoff led*/
void turn_off_led()
{
	LED_SWITCH_A0_LOW();
	LED_SWITCH_A1_LOW();
}
/**/
void pd1_adc()
{
}
/**/
void pd2_adc()
{
}
/**/
void pd3_adc()
{
}
#define aveNum 20
float adcBuffer;
float getAD_result()
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
		adcBuffer=aveRst;
		s365dRaw = adcBuffer * 1;
		s470dRaw =adcBuffer*1;
//		if(channnel_select_flag==channel_cs365)
//			{
//				if (s365dRawPrev < 0.0)
//				{
//					s365dRawPrev = s365dRaw;
//					kf_setInitial(s365dRaw);
//				}
//				else if (fabs(s365dRaw-s365dRawPrev)/s365dRawPrev > 0.15)	// reset filter when the current 2 measurement differ by more then 15%
//				{
//					kf_setInitial(s365dRaw);
//					kf_restart();
//				}
//				s365dRawPrev = s365dRaw;
//				return kf_calculation(s365dRaw);
//	    }
//			if(channnel_select_flag==channel_cs470)
//			{
//				if (s470dRawPrev < 0.0)
//				{
//					s470dRawPrev = s470dRaw;
//					kf_setInitial(s470dRaw);
//				}
//				else if (fabs(s470dRaw-s470dRawPrev)/s470dRawPrev > 0.15)	// reset filter when the current 2 measurement differ by more then 15%
//				{
//					kf_setInitial(s470dRaw);
//					kf_restart();
//				}
//				s470dRawPrev = s470dRaw;
//				return kf_calculation(s470dRaw);
//			}
//		else
//			{
//			 return aveRst;
//			}
			return aveRst;
}
float channel(u8 meaChannel)
{
		return getAD_result();
}
