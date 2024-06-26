#include<stdio.h>
#include<stdlib.h>
#include <time.h> 

int binary(float temp){
    if(temp >= 0.5) return 1;
    else return 0;
}

float train(float y1, float r1, float x, float trainrate){
    return x * (y1 - r1) * trainrate;
}

float randomFloat()
{
    float r = (float)rand()/(float)RAND_MAX;
    return r;
}

int main(){
    srand(time(NULL)); // 난수 발생기 초기화

    int epochs = 100; // 에포크 수를 늘림
    float w1_1, w1_2, x1_1, x1_2;
    float x1[10000], x2[10000], y1[10000];

    for(int i = 0; i < 10000; i++){
        x1[i] = randomFloat();
        x2[i] = randomFloat();
        if(x1[i] >= 0.5 || x2[i] >= 0.5) y1[i] = 1;
        else y1[i] = 0;
    }

    printf("Dataset is made\n");

    w1_1 = randomFloat();
    w1_2 = randomFloat();

    printf("%f\n", w1_1);
    printf("%f\n", w1_2);

    for(int b = 0; b < epochs; b++){
        for(int i = 0; i < 10000; i++){
            if(binary(x1[i] * w1_1 + x2[i] * w1_2) == y1[i]){
                continue;
            }
            else{
                w1_1 += train(y1[i], binary(x1[i] * w1_1 + x2[i] * w1_2), x1[i], 0.01); // 학습률을 조정
                w1_2 += train(y1[i], binary(x1[i] * w1_1 + x2[i] * w1_2), x2[i], 0.01); // 학습률을 조정
            }
        }
    }

    printf("%f\n", w1_1);
    printf("%f\n", w1_2);

    printf("Press x1\n");
    scanf("%f", &x1_1);
    printf("Press x2\n");
    scanf("%f", &x1_2);
    printf("result = %d", binary(x1_1 * w1_1 + x1_2 * w1_2));

    return 0;
}
