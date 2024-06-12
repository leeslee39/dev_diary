#include<stdio.h>
#include <stdlib.h> //srand, rand를 사용하기 위한 헤더파일
#include <time.h> // time을 사용하기 위한 헤더파일
void simulate(int*b,int*g);
int main(){
    int time, b = 0, g = 0;
    scanf("%d", &time);
    for(int i = time; i> 0; i—){
        simulate(&b, &g);
    }
    printf("boy is : %d, girl is %d, ratio: %f", b,g,(float)b/(float)g);
}
void simulate(int *b, int *g){
    while(1){
        if(rand() % 2 ==1){
            *b += 1;
            break;
        }
        else{
            *g += 1;
        }
    }
}