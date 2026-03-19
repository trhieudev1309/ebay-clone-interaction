// Real data fed from the server
let chats = [];
if (typeof initialChats !== 'undefined') {
    // Map backend DTOs to the expected structure
    chats = initialChats.map(c => ({
        id: (c.SenderId === currentUserId) ? c.ReceiverId : c.SenderId,
        name: c.Name,
        preview: c.LastMessage,
        online: false, // Could integrate with presence
        avatar: c.Avatar || 'https://i.pravatar.cc/150',
        messages: [] // Will fetch on demand
    }));
}

let currentChat = null;
const base = window.location.origin;

// Setup SignalR connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${base}/chatHub`)
    .withAutomaticReconnect()
    .build();

// Keep-alive settings
connection.serverTimeoutInMilliseconds = 60000;
connection.keepAliveIntervalInMilliseconds = 15000;

connection.start().then(async () => {
    console.log("BoxChat SignalR Connected!");
    try {
        await connection.invoke("ConnectUser", currentUserId);
    } catch {}
}).catch(err => {
    console.error("SignalR Connection Error: ", err);
});

connection.onreconnected(async (id) => {
    try {
        await connection.invoke("ConnectUser", currentUserId);
    } catch {}
});

// Receive live messages
connection.on("ReceiveMessage", async (messageData) => {
    // If the message belongs to the active chat
    if (currentChat && (messageData.senderId === currentChat.id || messageData.receiverId === currentChat.id)) {
        displayMessage(messageData);
    }
    
    // Update the sidebar preview
    const senderOrReceiverId = messageData.senderId === currentUserId ? messageData.receiverId : messageData.senderId;
    let chatItem = chats.find(c => c.id === senderOrReceiverId);
    
    if (chatItem) {
        chatItem.preview = messageData.content;
    } else {
        // It's a completely new conversation! Push it to the top.
        // The Hub sends senderName which we can use
        chatItem = {
            id: senderOrReceiverId,
            name: messageData.senderId === currentUserId ? "Me" : messageData.senderName,
            preview: messageData.content,
            online: false,
            avatar: 'https://i.pravatar.cc/150',
            messages: []
        };
        chats.unshift(chatItem); // Add to beginning
    }
    
    initChatList(); // re-render sidebar
});

// Initialize chat list
function initChatList() {
    const chatList = document.getElementById('chatList');
    chatList.innerHTML = '';

    chats.forEach(chat => {
        const chatItem = document.createElement('div');
        chatItem.className = 'chat-item';
        chatItem.innerHTML = `
            <img src="${chat.avatar}" alt="${chat.name}" class="chat-avatar">
            <div class="chat-info">
                <div class="chat-name">${chat.name}</div>
                <div class="chat-preview">${escapeHtml(chat.preview || '...')}</div>
            </div>
        `;
        chatItem.addEventListener('click', () => selectChat(chat.id));
        chatList.appendChild(chatItem);
    });

    // Keep active state visually consistent
    if (currentChat) {
        document.querySelectorAll('.chat-item').forEach((item, index) => {
            item.classList.toggle('active', chats[index].id === currentChat.id);
        });
    }
}

// Select chat
async function selectChat(chatId) {
    currentChat = chats.find(c => c.id === chatId);

    // Update sidebar active state
    document.querySelectorAll('.chat-item').forEach((item, index) => {
        item.classList.toggle('active', chats[index].id === chatId);
    });

    // Update header
    document.getElementById('chatName').textContent = currentChat.name;
    document.getElementById('chatStatus').textContent = currentChat.online ? 'Active now' : 'Offline';

    // Load actual message history from API
    await loadMessages();
}

async function loadMessages() {
    try {
        const res = await fetch(`${base}/Chat/GetMessages?senderId=${currentUserId}&receiverId=${currentChat.id}`);
        const messages = await res.json();
        
        const messagesContainer = document.getElementById('messagesContainer');
        messagesContainer.innerHTML = '';
        
        messages.forEach(displayMessage);
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    } catch (e) {
        console.error("Error loading messages:", e);
    }
}

function displayMessage(messageData) {
    const messagesContainer = document.getElementById('messagesContainer');
    const msgType = (messageData.senderId === currentUserId) ? "sent" : "received";
    
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${msgType}`;
    
    // Split the time "HH:mm dd/MM/yyyy" into parts if possible
    let timeParts = messageData.sentAt.split(" ");
    let timeStr = timeParts[0] || messageData.sentAt;
    let dateStr = timeParts[1] || "";

    messageDiv.innerHTML = `
        <div class="message-bubble">
            <div>${escapeHtml(messageData.content)}</div>
            <div class="message-time">${timeStr} | ${dateStr}</div>
        </div>
    `;
    messagesContainer.appendChild(messageDiv);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

// Send message
async function sendMessage() {
    const input = document.getElementById('messageInput');
    const text = input.value.trim();

    if (!text || !currentChat) return;

    try {
        await connection.invoke("SendMessage", currentUserId, currentChat.id, text);
        input.value = '';
        input.focus();
    } catch (e) {
        console.error("Error sending message:", e);
        alert("Failed to send message. Please try again.");
    }
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text ?? "";
    return div.innerHTML;
}

// Event listeners
document.getElementById('sendBtn').addEventListener('click', sendMessage);
document.getElementById('messageInput').addEventListener('keypress', (e) => {
    if (e.key === 'Enter') sendMessage();
});

// Initialize on page load
initChatList();
if (chats.length > 0) {
    selectChat(chats[0].id);
}