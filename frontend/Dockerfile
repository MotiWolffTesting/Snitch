# Build stage
FROM node:18-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm ci --only=production --silent

# Copy source code
COPY . .

# Build application
RUN npm run build

# Development stage
FROM node:18-alpine AS development
WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm install --silent

# Copy source code
COPY . .

# Expose port
EXPOSE 3000

# Start development server
CMD ["npm", "start"]

# Production stage
FROM nginx:alpine AS production

# Copy custom nginx config
COPY nginx.conf /etc/nginx/nginx.conf

# Copy built application
COPY --from=build /app/build /usr/share/nginx/html

# Add non-root user
RUN addgroup -g 1001 -S nodejs && adduser -S reactjs -u 1001

# Change ownership of nginx files
RUN chown -R reactjs:nodejs /var/cache/nginx && \
    chown -R reactjs:nodejs /var/log/nginx && \
    chown -R reactjs:nodejs /etc/nginx/conf.d && \
    touch /var/run/nginx.pid && \
    chown -R reactjs:nodejs /var/run/nginx.pid

# Switch to non-root user
USER reactjs

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:80/ || exit 1

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
