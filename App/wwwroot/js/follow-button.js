"use strict";

function setupFollowButton(button) {
  const targetuserId = button.dataset.userId;
  if (!targetuserId) return;

  button.disabled = true;

  fetch(`/api/FollowUser/IsFollowing/${targetuserId}`, {
    credentials: "include",
  })
    .then((response) => response.json())
    .then((isFollowing) => {
      updateButton(button, isFollowing);
      button.disabled = false;

      button.addEventListener("click", () => {
        button.disabled = true;

        fetch("/api/FollowUser/toggle", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          credentials: "include",
          body: JSON.stringify({ targetUserId: targetuserId }),
        })
          .then((response) => response.json())
          .then((data) => {
            if (data.errorCode === 0) {
              updateButton(button, data.isFollowing);
              alert(
                data.isFollowing
                  ? "フォローしました。"
                  : "フォロー解除しました。"
              );
            } else {
              alert("フォロー処理に失敗しました。");
            }
          })
          .catch((err) => {
            console.error("フォローエラー", err);
            alert("フォロー処理に失敗しました。");
          })
          .finally(() => {
            button.disabled = false;
          });
      });
    })
    .catch(() => {
      alert("フォロー状態の取得に失敗しました");
      button.disabled = true;
    });
}

function updateButton(button, isFollowing) {
  if (isFollowing) {
    button.textContent = "フォロー解除";
    button.classList.remove("btn-primary");
    button.classList.add("btn-secondary");
  } else {
    button.textContent = "フォローする";
    button.classList.remove("btn-secondary");
    button.classList.add("btn-primary");
  }
}
