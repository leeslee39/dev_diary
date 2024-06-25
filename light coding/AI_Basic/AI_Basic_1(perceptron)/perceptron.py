import numpy as np

TrainingRate = 0.01

def Training_rule(X, P, R):
    return (R-P)*X*TrainingRate ##트레이닝 룰 Error*X*Training Rate

def Dataset(num): ##데이터 셋 만드는 알고리즘
    output = []
    for i in range(num):
        data = np.random.rand(2)
        if data[0] >= 0.5 or data[1] >=0.5:
            label = 1
        else:
            label = 0
        output.append([data, label])
    return output

def Activation_Function(data):
    if data >= 0.5:
        return 1
    else:
        return 0

def main():
    TS = Dataset(10000) ##트레이닝 셋 만들기
    weight = np.random.rand(2)##가중치 랜덤으로 정하기
    for i in TS:
        print(i)
        perceptron = weight[0] * i[0][0] + weight[1] * i[0][1]##퍼셉트론의 시그마 연산
        weight[0] += Training_rule(i[0][0], Activation_Function(perceptron), i[1])##트레이닝 룰에 의한 가중치 업데이트 1
        weight[1] += Training_rule(i[0][1], Activation_Function(perceptron), i[1])##트레이닝 룰에 의한 가중치 업데이트 2
    print(weight)##최종 가중치 값
    evaluating(weight)


def evaluating(weight):##학습률 평가
    Test_time = 100 ##테스트 실행 횟수
    TS = Dataset(Test_time)
    error = 0
    for i in TS:
        perceptron = weight[0] * i[0][0] + weight[1] * i[0][1]
        predict = Activation_Function(perceptron)
        error += np.abs(predict - i[1])
    print(error/Test_time)##오차율


main()
