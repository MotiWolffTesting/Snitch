import axios from "axios";

const API_BASE_URL =
  process.env.REACT_APP_API_BASE_URL || "http://localhost:5243/api";

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
});

// Add request interceptor for logging
api.interceptors.request.use(
  (config) => {
    console.log("Making request to:", config.url);
    return config;
  },
  (error) => {
    console.error("Request error:", error);
    return Promise.reject(error);
  }
);

// Add response interceptor for logging
api.interceptors.response.use(
  (response) => {
    console.log("Response received:", response.status);
    return response;
  },
  (error) => {
    console.error(
      "Response error:",
      error.response?.status,
      error.response?.data
    );
    return Promise.reject(error);
  }
);

// People
export const getPeople = () => api.get("/People").then((res) => res.data);
export const getPerson = (id) =>
  api.get(`/People/${id}`).then((res) => res.data);
export const createPerson = (data) =>
  api.post("/People", data).then((res) => res.data);
export const deletePerson = (id) =>
  api.delete(`/People/${id}`).then((res) => res.data);

// Reports
export const getReports = () =>
  api.get("/Reports/recent").then((res) => res.data);
export const createReport = (data) =>
  api.post("/Reports", data).then((res) => res.data);
export const updateReport = (id, data) =>
  api.put(`/Reports/${id}`, data).then((res) => res.data);
export const deleteReport = (id) =>
  api.delete(`/Reports/${id}`).then((res) => res.data);
export const uploadReportsCsv = async (file) => {
  try {
    const formData = new FormData();
    formData.append("file", file);

    const response = await axios.post(
      `${API_BASE_URL}/Reports/import-csv`,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
        transformRequest: [(data) => data], // Prevent Axios from transforming FormData
      }
    );

    return response.data;
  } catch (error) {
    console.error("Error uploading CSV:", error);
    throw error;
  }
};

// Alerts
export const getAlerts = () =>
  api.get("/Alerts/active").then((res) => res.data);
export const deleteAlert = (id) =>
  api.delete(`/Alerts/${id}`).then((res) => res.data);

// Analysis Preference
export const getAnalysisPreference = () =>
  api.get("/AnalysisPreference").then((res) => res.data);
export const setAnalysisPreference = (useOpenAI) =>
  api.post("/AnalysisPreference", useOpenAI, {
    headers: {
      'Content-Type': 'application/json'
    }
  }).then((res) => res.data);

// Add more as needed
