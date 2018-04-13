'use strict'

import path from 'path'
import os from 'os'
import { app, BrowserWindow } from 'electron'

/**
 * Set `__static` path to static files in production
 * https://simulatedgreg.gitbooks.io/electron-vue/content/en/using-static-assets.html
 */
if (process.env.NODE_ENV !== 'development') {
  global.__static = require('path')
    .join(__dirname, '/static')
    .replace(/\\/g, '\\\\')
}

let mainWindow
const winURL =
  process.env.NODE_ENV === 'development'
    ? `http://localhost:9080`
    : `file://${__dirname}/index.html`

function createWindow() {
  /**
   * Initial window options
   */
  mainWindow = new BrowserWindow({
    height: 563,
    useContentSize: true,
    width: 1000
  })

  mainWindow.loadURL(winURL)
  mainWindow.maximize()
  mainWindow.on('closed', () => {
    mainWindow = null
  })
}
function startApi() {
  const proc = require('child_process').spawn
  //  run server
  let workingDir = path.resolve(__dirname, '../api/bin/dist/win/')
  let apipath = path.resolve(workingDir, 'FastSQL.API.exe')
  if (os.platform() === 'darwin') {
    apipath = path.resolve(workingDir, 'FastSQL.API')
  }
  console.log(`Starting ${apipath}`)
  const apiProcess = proc(apipath, {
    cwd: workingDir,
    env: {
      ASPNETCORE_URLS: 'http://localhost:7001'
    }
  })

  apiProcess.stdout.on('data', data => {
    console.log(`stdout: ${data}`)
    if (mainWindow == null) {
      createWindow()
    }
  })
}
if (process.env.NODE_ENV === 'production') {
  process.env.BACKEND = 'http://localhost:7001'
  app.on('ready', startApi)
} else {
  process.env.BACKEND = 'http://localhost:63923'
  app.on('ready', createWindow)
}

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit()
  }
})

app.on('activate', () => {
  if (mainWindow === null) {
    createWindow()
  }
})

/**
 * Auto Updater
 *
 * Uncomment the following code below and install `electron-updater` to
 * support auto updating. Code Signing with a valid certificate is required.
 * https://simulatedgreg.gitbooks.io/electron-vue/content/en/using-electron-builder.html#auto-updating
 */

/*
import { autoUpdater } from 'electron-updater'

autoUpdater.on('update-downloaded', () => {
  autoUpdater.quitAndInstall()
})

app.on('ready', () => {
  if (process.env.NODE_ENV === 'production') autoUpdater.checkForUpdates()
})
 */
