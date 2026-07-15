import axios from 'axios'

/**
 * Axios 实例，预配置 baseURL 和请求拦截器。
 */
const apiClient = axios.create({
  baseURL: 'https://localhost:5001/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// 响应拦截器：统一错误处理
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const message = error.response?.data?.message || error.message || '请求失败'
    console.error(`[API Error] ${message}`)
    return Promise.reject(error)
  }
)

export default apiClient
