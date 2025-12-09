# Iskra LMS

> 🚧 **Status:** Work in Progress (Active Development)

**Iskra** is a next-generation Learning Management System (LMS) built on **ASP.NET Core 9**.

Unlike traditional rigid educational platforms, Iskra is architected as a **Modular Monolith**. It combines the simplicity of a monolithic deployment with the flexibility of microservices. The core philosophy is **"Everything is a Module."** Authentication, Database Providers (MariaDB/PostgreSQL), and Business Features (Courses, Users) are all implemented as decoupled plugins that are loaded dynamically at runtime.

### 🏗 Architecture Highlights

-   **Modular Monolith:** Logical separation of features with physical deployment simplicity.
-   **Pluggable Infrastructure:** Database providers are swappable modules. Configuration is centralized but scoped.
-   **Clean Architecture:** Strict separation of concerns (Domain, Application, Infrastructure).
-   **Dynamic Loading:** Modules are loaded from build artifacts, keeping the Core kernel lightweight.

### 🤝 Contributing

We welcome contributions from the .NET community!
Since the project is in active development, there are many opportunities to shape the architecture and feature set.

If you want to help:
1.  Create a feature branch.
2.  Submit a Pull Request.

Please check the Issues tab for current tasks or open a discussion if you have architectural ideas.

---

## 📜 License

This project is licensed under the **Iskra Community License (ICL)**.

| Use Case | Status | Condition |
| :--- | :--- | :--- |
| **Non-Commercial** | ✅ Free | Unlimited users (Education, Non-profit, Personal). |
| **Small Business** | ✅ Free | Up to **500 active users** total. |
| **Enterprise** | 💼 Paid | Required if you exceed 500 users. |

**Important:**
1. **Forks & Derivatives:** If you modify this code, your version is still subject to the 500-user limit.
2. **No Loopholes:** Splitting users across multiple servers to bypass limits is prohibited.
3. **Updates:** The author reserves the right to update license terms at any time.

Read the full [LICENSE](LICENSE) for details.