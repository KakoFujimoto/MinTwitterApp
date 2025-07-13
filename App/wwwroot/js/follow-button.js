"use strict";

document.addEventListener("DOMContentLoaded", function () {
  const buttons = document.querySelectorAll(".followBtn");

  buttons.forEach((button) => {
    const targetUserId = button.dataset.userId;

    // フォロー状態を取得
    fetch(`/api/FollowUser/IsFollowing/${targetUserId}`, {
      credentials: "include",
    })
      .then((response) => response.json())
      .then((isFollowing) => {
        updateButton(button, isFollowing);
        button.disabled = false;
      })
      .catch(() => {
        alert("フォロー状態の取得に失敗しました");
        button.disabled = true;
      });

    button.addEventListener("click", () => {
      button.disabled = true;

      fetch("/api/FollowUser/toggle", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify({ targetUserId: targetUserId }),
      })
        .then((response) => response.json())
        .then((data) => {
          if (data.errorCode === 0) {
            updateButton(button, data.isFollowing);
            alert(
              data.isFollowing ? "フォローしました。" : "フォロー解除しました。"
            );
          } else {
            alert("フォロー処理に失敗しました。");
          }
          button.disabled = false;
        })
        .catch((error) => {
          console.error("フォロー処理中にエラーが発生", error);
          alert("フォロー処理に失敗しました。");
          button.disabled = false;
        });
    });
  });

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
});
