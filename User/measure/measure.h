#ifndef __MEASURE_H__
#define __MEASURE_H__

#include "main.h"
#include "kf.h"
#define channel_ct365  1
#define channel_cs365  2
#define channel_cs470  3
#define channel_cs410  4
#define channel_cs470_low 5
#define channel_cs347 6
#define channel_cs365_a 7
#define channel_cs365_b 8
#define channel_cs365_c 9
#define channel_cs365_d 10



void turn_on_led_d7(void);
void turn_on_led_d8(void);
void turn_on_led_d9(void);
void turn_on_led_d10(void);
void turn_off_led(void);
void pd1_adc(void);
void pd2_adc(void);
void pd3_adc(void);
float getAD_result(void);
float channel(u8 meaChannel);

#endif
