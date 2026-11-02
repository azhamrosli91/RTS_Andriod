import http from 'node:http';
import {createReadStream} from 'node:fs';
import {stat} from 'node:fs/promises';
import {fileURLToPath} from 'node:url';
import path from 'node:path';
const root=path.resolve(fileURLToPath(new URL('./public/',import.meta.url)));
const apk=path.resolve(process.env.APK_PATH || fileURLToPath(new URL('./downloads/rts-strategy.apk',import.meta.url)));
const port=Number(process.env.PORT || 8080);
const apkInfo=async()=>{try {const s=await stat(apk);return s.isFile()&&s.size>0?s:null;}catch{return null;}};
const types={'.html':'text/html; charset=utf-8','.css':'text/css; charset=utf-8','.js':'text/javascript; charset=utf-8','.svg':'image/svg+xml'};
http.createServer(async(req,res)=>{
 res.setHeader('X-Content-Type-Options','nosniff');res.setHeader('Referrer-Policy','strict-origin-when-cross-origin');
 res.setHeader('Content-Security-Policy',"default-src 'self'; style-src 'self'; script-src 'self'; img-src 'self' data:; object-src 'none'; base-uri 'none'; frame-ancestors 'none'");
 if(!['GET','HEAD'].includes(req.method)){res.writeHead(405,{Allow:'GET, HEAD'});res.end();return;}
 let pathname;try{pathname=decodeURIComponent(new URL(req.url,'http://localhost').pathname);}catch{res.writeHead(400);res.end();return;}
 if(pathname==='/healthz'){res.writeHead(200,{'Content-Type':'text/plain'});res.end(req.method==='HEAD'?undefined:'ok');return;}
 if(pathname==='/api/release'){const s=await apkInfo();res.writeHead(200,{'Content-Type':'application/json','Cache-Control':'no-store'});res.end(req.method==='HEAD'?undefined:JSON.stringify({available:!!s,bytes:s?.size||0,updatedAt:s?.mtime.toISOString()||null,url:s?'/download':null}));return;}
 let file;
 if(pathname==='/download'){
  const s=await apkInfo();if(!s){res.writeHead(404,{'Content-Type':'text/plain','Cache-Control':'no-store'});res.end('No Android APK has been published yet.');return;}
  file=apk;res.setHeader('Content-Type','application/vnd.android.package-archive');res.setHeader('Content-Disposition','attachment; filename="rts-strategy.apk"');res.setHeader('Cache-Control','no-store');
 }else{
  file=path.resolve(root,'.'+(pathname==='/'?'/index.html':pathname));
  if(!file.startsWith(root+path.sep)){res.writeHead(403);res.end();return;}
  res.setHeader('Content-Type',types[path.extname(file)]||'application/octet-stream');res.setHeader('Cache-Control','no-cache');
 }
 try{const s=await stat(file);if(!s.isFile())throw Error();res.setHeader('Content-Length',s.size);if(req.method==='HEAD'){res.end();return;}const stream=createReadStream(file);stream.on('error',()=>res.destroy());stream.pipe(res);}catch{res.writeHead(404,{'Content-Type':'text/plain'});res.end('Not found');}
}).listen(port,'0.0.0.0',()=>console.log(`RTS landing page listening on ${port}`));
