/*
 * kf.h
 *
 *  Created on: 2019Äê4ÔÂ20ÈÕ
 *      Author: tim
 */

#ifndef __KF_H__
#define __KF_H__

extern void kf_restart(void);
extern float kf_init(float origValue);
extern void  kf_setInitial(float value);
extern float kf_iterate(float origValue);
extern float kf_calculation(float origValue);

#endif /* __KF_H__ */
