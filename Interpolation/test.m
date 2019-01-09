clear; clc; close;
[X,Y]=bspline(0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5);
hold on
for i=1:length(X)
    plot(X{i},Y{i});
end
