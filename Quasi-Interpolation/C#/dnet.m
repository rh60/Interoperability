ff=fullfile(pwd,'BSpline.dll');
a=NET.addAssembly(ff);
bs=MMP.BSpline([0, 0, 0, 1, 1, 1]);
pp=bs.Bases();

c=pp.Item(0);
c.ToArray()