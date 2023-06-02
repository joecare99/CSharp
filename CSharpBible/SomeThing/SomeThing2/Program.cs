using ConsoleDisplay.View;MyConsole c=new();Rnd r=new();const int 
Z=1<<11,u=(0x13<<1)+1,O=u&-u,D=O<<((O<<O)+1),B=0x597b;var C=" _|";int[]l0=new 
int[]{0,O-4,u-O,-O},lQ=new int[]{O,u,-O,-u},E=new int[Z];void Q(int x)=>c.Write
((x<D)?$"{C[((B>>(x++<<O))&3)-1]}{C[((B>>(x<<O))&3)-1]}":$"{C}{C}{C}\r\n"[x..]);int
h=0,w=D-D,P,A,S,L=Z-Z,a,l=0;int[]ll=new int[Z],Il=new int[4];E[0]=D;S=P=u*u-O;E
[S]=Z+2;while (w!=0||L>=h){A=S;S=P;a=E[A];w=0;foreach(int i in l0){P=lQ[i&3]+A;
if((P>=0)&&(P<u*u)&&(i!=(P%u))&&(a&Z)!=(E[P]&Z))Il[w++]=i;}if(w!=0){var i=Il[r.
Next(w)]&3;P=lQ[i]+A;E[A]=a|(O<<i);E[P]|=Z|(O<<((i+2)%4));ll[L++]=P;}else if(L
>=h)P=ll[h++];}Q(D-2);for(A=O;A<u;A++)Q(D>>O);Q(D+O);for(A=O;A<u+O;A++){for(
P=O;P<u+O;P++)Q(E[l++] & 6);Q(D);}
