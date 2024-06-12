#include<stdio.h>
int main(){
    int arr[10];
    printf("Enter first set:\n");
    for(int i = 0; i < 10; i++){
        scanf("%d", &arr[i]);
        arr[i] = arr[i]*2;
    }
    printf("Enter secend set:\n");
    for(int i = 0; i < 10; i++){
        int scan;
        scanf("%d", &scan);
        for(int i = 0; i < 10; i++){
            if(arr[i]/2 == scan && arr[i]%2 != 1){
                arr[i] += 1;
            }
        }
    }
    printf("The intersection set:\n");
    for(int i = 0; i < 10; i++){
        if(arr[i] % 2 == 1){
            printf(" %d", (arr[i]-1)/2);
        }
    }
}