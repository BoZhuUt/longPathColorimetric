/*
 * kf.c
 *
 *  Created on: 2019Äê4ÔÂ20ÈÕ
 *      Author: tim
 */
#include <stdio.h>
#include <stdlib.h>
//#include "basetypes.h"
#include "kf.h"
typedef char Bool;
#define FALSE   ( 0 )
#define TRUE    ( !FALSE )
static float kf_Po=1.0;
static float kf_Q=0.1;
static float kf_X0=2000.0;
static float kf_R=0.0001;
static Bool kf_started=FALSE;

static float kf_output;
static float P_k;
static float Q_k;
static float K_k;
static float X_k;

void kf_restart(void)
{
	kf_started = FALSE;
}

void kf_setInitial(float value)
{
	kf_X0 = value;
}

float kf_init(float origValue)
{
    kf_output = kf_X0;
    P_k = kf_Q + kf_Po;
    K_k = P_k/(P_k + kf_R);
    X_k = kf_output + K_k*(origValue-kf_output);
    Q_k = (1 - K_k) * P_k;

    return kf_output;
}

float kf_iterate(float origValue)
{
    kf_output = X_k;
    P_k = Q_k;
    K_k = P_k/(P_k + kf_R);
    X_k = kf_output + K_k*(origValue-kf_output);
    Q_k = (1 - K_k) * P_k;

    return kf_output;
}

float kf_calculation(float origValue)
{
    float filterValue;

    if (kf_started == FALSE)
    {
        filterValue = kf_init(origValue);
        kf_started = TRUE;
    }
    else
    {
        filterValue = kf_iterate(origValue);
    }

    return filterValue;
}
