import axios from "axios";
import CryptoJS from "crypto-js";

const API_URL = process.env.REACT_APP_API_URL || "http://localhost:5243/api";
const ENCRYPTION_KEY =
  process.env.REACT_APP_ENCRYPTION_KEY || "your-secure-key";

// Encryption utilities
export const encryptData = (data) => {
  return CryptoJS.AES.encrypt(JSON.stringify(data), ENCRYPTION_KEY).toString();
};

export const decryptData = (encryptedData) => {
  const bytes = CryptoJS.AES.decrypt(encryptedData, ENCRYPTION_KEY);
  return JSON.parse(bytes.toString(CryptoJS.enc.Utf8));
};

// Authentication functions
export const register = async (username, password) => {
  try {
    const response = await axios.post(`${API_URL}/auth/register`, {
      username,
      password,
    });
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.message || "Registration failed");
  }
};

export const login = async (username, password) => {
  try {
    const response = await axios.post(`${API_URL}/auth/login`, {
      username,
      password,
    });
    if (response.data.token) {
      localStorage.setItem("user", encryptData(response.data));
      return response.data;
    }
    return null;
  } catch (error) {
    throw new Error(error.response?.data?.message || "Login failed");
  }
};

export const logout = () => {
  localStorage.removeItem("user");
};

export const getCurrentUser = () => {
  const userStr = localStorage.getItem("user");
  if (!userStr) return null;

  try {
    return decryptData(userStr);
  } catch (error) {
    logout();
    return null;
  }
};

export const isAuthenticated = () => {
  const user = getCurrentUser();
  return user && user.token;
};

export const isAdmin = () => {
  const user = getCurrentUser();
  return user && user.isAdmin;
};

export const isApproved = () => {
  const user = getCurrentUser();
  return user && user.isApproved;
};

// Add auth token to all requests
axios.interceptors.request.use(
  (config) => {
    const user = getCurrentUser();
    if (user && user.token) {
      config.headers.Authorization = `Bearer ${user.token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);
